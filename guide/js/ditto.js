// github info
const GITHUB_REPO = 'jacobdufault/fullinspector';
const GITHUB_DOC_URL = 'https://github.com/jacobdufault/fullinspector/edit/gh-pages/guide';

// markdown files
const MARKDOWN_INDEX = 'index.md'
const MARKDOWN_SIDEBAR = 'sidebar.md'

// selectors
const ERROR_SELECTOR = '#error';
const CONTENT_SELECTOR = '.content';
const SIDEBAR_SELECTOR = '#sidebar';
const EDIT_SELECTOR = '#edit';
const LOADING_SELECTOR = '#loading';
const SEARCH_SELECTOR = '#search';
const SEARCH_RESULTS_CLASS = '.search_results';
const FRAGMENTS_CLASS = '.fragments';
const FRAGMENT_CLASS = '.fragment';

// config
const CACHE_EXPIRATION_MINUTES = 5;
const MAX_SEARCH_MATCHES_PER_FILE = 15;
const FULL_FUZZY_SEARCH = false;

$(document).ready(() => {
  // Always clear the cache on startup so we don't have stale data.
  lscache.flush();

  init_sidebar();
  init_edit_button();

  // page router
  router();
  $(window).on('hashchange', router);
});

function init_sidebar() {
  get_file(MARKDOWN_SIDEBAR)
    .then(/* success */(data) => {
      var marked_data = marked(data);
      $(SIDEBAR_SELECTOR).html(marked_data);

      init_searchbar();
    }, /* error */() => {
      alert('Sidebar failed to load');
    });
}

function init_edit_button() {
  $(EDIT_SELECTOR).show();
  $(EDIT_SELECTOR).on('click', function () {
    var hash = location.hash.replace('#', '/');

    if (hash === '') {
      hash = '/' + MARKDOWN_INDEX.replace('.md', '');
    }

    hash = hash.split('?')[0];

    // open is better than redirecting, as the previous page history
    // with redirect is a bit messed up
    window.open(GITHUB_DOC_URL + hash + '.md');
  });
}

function init_searchbar() {
  var sidebar = $(SIDEBAR_SELECTOR).html();
  var match = '[ditto:searchbar]';

  // html input searchbar
  var search = '<input name="' + SEARCH_SELECTOR + '"';
  search = search + ' type="search">';

  // replace match code with a real html input search bar
  sidebar = sidebar.replace(match, search);
  let search_element = $(SIDEBAR_SELECTOR).html(sidebar);

  // add search listener and set initial searchbar value
  $('input[name=' + SEARCH_SELECTOR + ']').keyup(searchbar_listener);
  if (location.hash.indexOf('#search=') >= 0)
    $('input[name=' + SEARCH_SELECTOR + ']')[0].value = location.hash.replace('#search=', '');
}

function build_text_matches_html(match) {
  let file_content = match.content;

  let newline_regex = /[\n\r]/g;
  // Have an initial newline at start to remove some edge cases.
  let newline_indices = [ 0 ];
  while (true) {
    let match = newline_regex.exec(file_content);
    if (!match)
      break;
    newline_indices.push(match.index);
  }

  function get_match_fragment(fragment) {
    const CONTEXT_LINES = 3;

    let before_newline_index = 0;
    while (before_newline_index < newline_indices.length &&
      newline_indices[before_newline_index] < fragment.match_start)
      ++before_newline_index;

    let match_end = fragment.match_start + fragment.match_length;
    let after_newline_index = before_newline_index + 1;

    before_newline_index -= CONTEXT_LINES + 1;
    after_newline_index += CONTEXT_LINES + 1;

    let newline_start = newline_indices[Math.max(before_newline_index - 1, 0)];
    let newline_end = newline_indices[Math.min(after_newline_index, newline_indices.length - 1)];

    return {
      start: file_content.substr(newline_start, fragment.match_start - newline_start),
      match: file_content.substr(fragment.match_start, fragment.match_length),
      end: file_content.substr(match_end, newline_end - match_end)
    }
  }

  let class_name = FRAGMENTS_CLASS.replace('.', '');

  let html = '<ul class="' + class_name + '">';
  console.log('There are ' + match.matches.length + ' matches');
  let current = 0;
  for (let entry of match.matches) {
    if (current++ > MAX_SEARCH_MATCHES_PER_FILE) break;
    let fragment = get_match_fragment(entry);
    html += '<li class="' + FRAGMENT_CLASS.replace('.', '') + '">';
    html += '<pre><code> ';
    html += '<p>' + $('#hide').text(fragment.start).html() + '</p>';
    html += '<p class="highlight">' + $('#hide').text(fragment.match).html() + '</p>';
    html += '<p>' + $('#hide').text(fragment.end).html() + '</p>';
    html += ' </code></pre></li>';
  }
  html += '</ul>';

  if (current > MAX_SEARCH_MATCHES_PER_FILE) {
    let hidden_count = match.matches.length - MAX_SEARCH_MATCHES_PER_FILE;
    html = '<p><i>(' + hidden_count + ' additional ' + (hidden_count > 1 ? 'matches' : 'match') + ' hidden)</i><p>' + html;
  }

  return html;
}

function build_result_matches_html(matches) {
  var html = '';
  var class_name = SEARCH_RESULTS_CLASS.replace('.', '');

  html += '<ul class="' + class_name + '">';
  for (var i = 0; i < matches.length; i++) {
    var url = matches[i].file;

    if (url == MARKDOWN_SIDEBAR) {
      console.log('Skipping ' + url);
      continue;
    }

    var hash = '#' + url.replace('.md', '');
    var path = window.location.origin + '/' + hash;

    //html += '<li class="link">' + url + '</li>'

    // url/sidebar.md
    // url/docs/sidebar.md
    var file = url.replace('.md', '');

    // /url/
    // /fullname/url/
    var toRemove = location.pathname;

    // /url
    // /fullname/url
    toRemove = toRemove.substring(0, toRemove.length - 1);

    var destination = trim_start_with_end(file, toRemove);
    if (destination[0] == '/') destination = destination.slice(1);
    destination = '#' + destination;

    var display_name = trim_start_with_end(file, toRemove);
    if (display_name.startsWith('docs/'))
      display_name = display_name.substr('docs/'.length);
    display_name = display_name.replace(/_/g, ' ');

    html += '<a href="' + destination + '">' + display_name + '</a>';

    var match = build_text_matches_html(matches[i]);
    html += match;
  }
  html += '</ul>';

  return html;
}

// Example input/output
//  trim_start_with_end('aabccd', 'foo/aab') -> 'ccd'
function trim_start_with_end(initial, ending) {
  function starts_with(str, starting) {
    return str.indexOf(starting) == 0;
  }

  for (var i = 0; i < ending.length; ++i) {
    var sub = ending.substring(i);
    if (starts_with(initial, sub)) {
      return initial.substring(sub.length);
    }
  }

  return initial;
}

function search(query) {
  // Extremely aggressive fuzzy search (between every token).
  if (FULL_FUZZY_SEARCH) {
    function intersperse(string, token) {
      let result = '';
      let first = true;
      for (let c of string) {
        result += c;
        if (!first)
          result += token;
        first = false;
      }
      return result;
    }
    query = intersperse(query, '.*?');
  }
  // Simple fuzzy search (in spaces)
  else {
    query = query.replace(' ', '.*?');
  }

  console.log('Running search for ' + query);
  run_search(query).then((results) => {
    var results_html = '<h1>Search Results</h1>';

    if (results.length) {
      hide_error();
      results_html += build_result_matches_html(results);
    } else {
      show_error('Oops! No matches found');
    }

    $(CONTENT_SELECTOR).html(results_html);
  });
}

function searchbar_listener(event) {
  var q = $('input[name=' + SEARCH_SELECTOR + ']').val();
  if (q !== '') {
    location.hash = '#search=' + q;
  }
}

function replace_symbols(text) {
  // replace symbols with underscore
  return text.replace(/[&\/\\#,+=()$~%.'":*?<>{}\ \]\[]/g, '_').toLowerCase();
}

function li_create_linkage(li_tag, header_level) {
  // add custom id and class attributes
  html_safe_tag = replace_symbols(li_tag.text());
  li_tag.attr('id', html_safe_tag);
  //li_tag.attr('class', 'link');

  make_link(li_tag);

  // add click listener - on click scroll to relevant header section
  $(CONTENT_SELECTOR + ' li#' + li_tag.attr('id')).click(function (event) {
    event.preventDefault();

    // scroll to relevant section
    var header = find_header_at_level(li_tag.attr('id'), header_level);
    scroll_to_header(header, /*animate:*/true);
  });
}

function find_header(anchor_name) {
  // anchor name, is, ie, 'my_custom_title' which has been created from 'My custom title'
  for (var i = 1; i <= 6; i++) {

    var header = find_header_at_level(anchor_name, i);
    if (header.length == 0) continue;

    return header;
  }
}

function find_header_at_level(anchor_name, level) {
  return $(CONTENT_SELECTOR + ' h' + level + '.' + anchor_name);
}

function scroll_to_header(header, animate) {
  $('html, body').animate({
    scrollTop: header.offset().top
  }, animate ? 200 : 0);

  // highlight the relevant section
  header.css('color', '#4682BE');
  header.css('text-decoration', 'underline');
}

function make_link(element) {
  var title = $(element).text();
  var anchor_name = replace_symbols(title);
  var link = window.location.href.split('?')[0] + '?' + anchor_name;

  $(element).text('');
  $(element).append('<a href=' + link + '>' + title + '</a>');
}

function create_page_anchors() {
  // create page anchors by matching li's to headers
  // if there is a match, create click listeners
  // and scroll to relevant sections

  for (var i = 1; i <= 6; i++) {
    // parse all headers
    var headers = [];
    $(CONTENT_SELECTOR + ' h' + i).map(function () {
      headers.push($(this).text());

      var id = replace_symbols($(this).text());
      $(this).addClass(id);

      make_link(this);
      $(this).children().css('color', 'inherit');
    });

    // parse and set links between li and h2
    $(CONTENT_SELECTOR + ' ul li').map(function () {
      for (var j = 0; j < headers.length; j++) {
        if (headers[j] === $(this).text()) {
          li_create_linkage($(this), i);
        }
      }
    });
  }
}

function show_error(err_msg) {
  $(ERROR_SELECTOR).html(err_msg);
  $(ERROR_SELECTOR).show();
}

function hide_error() {
  $(ERROR_SELECTOR).hide();
}

function set_loading_visible(visible) {
  // show
  if (visible) {
    $(LOADING_SELECTOR).show();
    $(CONTENT_SELECTOR).html('');  // clear content

    // infinite loop until clearInterval() is called on loading
    loading = setInterval(function () {
      $(LOADING_SELECTOR).fadeIn(1000).fadeOut(1000);
    }, 2000);
  }

  // hide
  else {
    clearInterval(loading);
    delete loading;
    $(LOADING_SELECTOR).hide();
  }
}

function get_file(path) {
  return new Promise((resolve, reject) => {
    lscache.enableWarnings(true);

    var data = lscache.get(path);
    if (data) {
      console.log('[CACHE HIT]: ' + path);
      resolve(data);
    }

    else {
      console.log('[CACHE MISS]: ' + path);
      $.get(path, function (data) {
        lscache.set(path, data, CACHE_EXPIRATION_MINUTES);
        resolve(data);
      }).fail(reject);
    }
  });
}

function run_search(search_string) {
  // Collects all of the matches from a given regex.
  function match_all(file, contents, regex) {
    function build_match(match) {
      return { match_start: match.index, match_length: match[0].length };
    }

    let matches = [];

    var match;
    if (regex.global) {
      while (match = regex.exec(contents)) {
        matches.push(build_match(match));
      }
    } else {
      if (match = regex.exec(contents)) {
        matches.push(build_match(match));
      }
    }

    return {
      file: file,
      content: contents,
      matches: matches
    };
  }

  return get_file(MARKDOWN_SIDEBAR).then(get_links_in_markdown).then((file_paths) => {
    let to_search = [];
    let promises = [];

    for (let markdown_file of file_paths) {
      let file_path = markdown_file + '.md';
      let promise =
        get_file(file_path)
          .then((content) => to_search.push({ file: file_path, content: content }));
      promises.push(promise);
    }

    return Promise.all(promises).then(() => to_search);
  }).then((to_search) => {
    console.log('Searching for ' + search_string);
    let searcher = new RegExp(search_string, 'ig');
    let result = [];

    for (let item of to_search) {
      let matches = match_all(item.file, item.content, searcher);
      if (matches.matches.length > 0)
        result.push(matches);
    }

    console.log('Result');
    console.log(result);
    return result;
  });
}

function get_links_in_markdown(markdown_content) {
  let result = [];

  let url_regex = /\(#(docs\/.*?)\)/g;
  while (true) {
    let match = url_regex.exec(markdown_content);
    if (!match)
      break;

    result.push(match[1]);
  }

  return result;
}

function page_getter() {
  // When we fetch a new page we want to scroll back to the
  // top of the window, otherwise we may show the user the
  // middle of an existing document.
  window.scrollTo(0, 0);

  var request_path = '';
  var scroll_target = '';

  // If we don't have a hash, then use the default page.
  if (location.hash === '') {
    request_path = MARKDOWN_INDEX;
    scroll_target = location.search.substr(1);
  }

  // There's a hash.
  else {
    // Skip #
    var query = location.hash.substr('#'.length);

    // There is a ? in the query. Split it up.
    if (query.indexOf('?') >= 0) {
      request_path = query.substr(0, query.indexOf('?'));
      scroll_target = query.substr(query.indexOf('?') + 1)
    } else {
      request_path = query;
      scroll_target = '';
    }

    // Add markdown extension.
    request_path = request_path + '.md';
  }

  // otherwise get the markdown and render it
  hide_error();
  set_loading_visible(true);

  get_file(request_path).then(
    /* success */(data) => {
      // compile the data
      data = marked(data);
      $(CONTENT_SELECTOR).html(data);

      // We do not create anchors on the main page. It breaks things.
      if (request_path != MARKDOWN_INDEX)
        create_page_anchors();

      // Highlight code.
      $('pre code').each(function (i, block) {
        if (typeof hljs !== 'undefined') {
          hljs.highlightBlock(block);
        }
      });

      // move to target
      if (scroll_target) {
        var header = find_header(scroll_target);
        if (header)
          scroll_to_header(header, /*animate:*/false);
      }
    },
    /* error */() => {
      show_error('Opps! File not found!');
    })
    .then( /* always */() => {
      set_loading_visible(false);
    });

  // hide loading after five seconds... sometimes the *always* function is not invoked
  setTimeout(function () { set_loading_visible(false); }, 5000);
}

function router() {
  let hash = location.hash;
  if (hash.slice(1, 7) !== 'search') {
    page_getter();
  } else {
    let search_string = hash.replace('#search=', '');
    search(search_string);
  }
}

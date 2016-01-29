var ditto = {
  // selectors
  content_id: "#content",
  sidebar_id: "#sidebar",
  edit_id: "#edit",
  loading_id: "#loading",
  error_id: "#error",
  search_name: "#search",
  search_results_class: ".search_results",
  fragments_class: ".fragments",
  fragment_class: ".fragment",

  // github specifics
  github_username: "jacobdufault",
  github_repo: "fullinspector",

  // highlight markdown information
  index: "index.md",
  sidebar_file: "sidebar.md",

  // where the docs are actually stored on github; this enables the edit button.
  base_url: "https://github.com/jacobdufault/fullinspector/edit/gh-pages/guide",

  // initialize function
  run: initialize
};

function initialize() {
  // Always clear the cache on startup so we don't have stale data.
  lscache.flush();

  init_sidebar();
  init_edit_button();

  // page router
  router();
  $(window).on('hashchange', router);
}

function init_sidebar() {
  get_file(ditto.sidebar_file,
    /*processor:*/function (data) {
      var marked_data = marked(data);
      $(ditto.sidebar_id).html(marked_data);

      init_searchbar();
    },
    /*error:*/function () {
      alert("Oops! can't find the sidebar file to display!");
    });
}


function init_edit_button() {
  $(ditto.edit_id).show();
  $(ditto.edit_id).on("click", function () {
    var hash = location.hash.replace("#", "/");

    if (hash === "") {
      hash = "/" + ditto.index.replace(".md", "");
    }

    hash = hash.split('?')[0];

    // open is better than redirecting, as the previous page history
    // with redirect is a bit messed up
    window.open(ditto.base_url + hash + ".md");
  });
}

function init_searchbar() {
  var sidebar = $(ditto.sidebar_id).html();
  var match = "[ditto:searchbar]";

  // html input searchbar
  var search = "<input name='" + ditto.search_name + "'";
  search = search + " type='search'";
  search = search + " results='10'>";

  // replace match code with a real html input search bar
  sidebar = sidebar.replace(match, search);
  $(ditto.sidebar_id).html(sidebar);

  // add search listener
  $("input[name=" + ditto.search_name + "]").keydown(searchbar_listener);
}

function build_text_matches_html(fragments) {
  var html = "";
  var class_name = ditto.fragments_class.replace(".", "");

  html += "<ul class='" + class_name + "'>";
  for (var i = 0; i < fragments.length; i++) {
    var fragment = fragments[i].fragment.replace("/[\uE000-\uF8FF]/g", "");
    html += "<li class='" + ditto.fragment_class.replace(".", "") + "'>";
    html += "<pre><code> ";
    fragment = $("#hide").text(fragment).html();
    html += fragment;
    html += " </code></pre></li>";
  }
  html += "</ul>";

  return html;
}

function build_result_matches_html(matches) {
  var html = "";
  var class_name = ditto.search_results_class.replace(".", "");

  html += "<ul class='" + class_name + "'>";
  for (var i = 0; i < matches.length; i++) {
    var url = matches[i].path;
    if (url.indexOf("sidebar.md") >= 0 ||
      url.indexOf("index.md") >= 0) {

      console.log("Search - ignoring " + url);
      continue;
    }

    if (url !== ditto.sidebar_file) {
      var hash = "#" + url.replace(".md", "");
      var path = window.location.origin + "/" + hash;

      //html += "<li class='link'>" + url + "</li>"

      // url/sidebar.md
      // url/docs/sidebar.md
      var file = url.replace(".md", "");

      // /url/
      // /fullname/url/
      var toRemove = location.pathname;

      // /url
      // /fullname/url
      toRemove = toRemove.substring(0, toRemove.length - 1);

      var destination = trim_start_with_end(file, toRemove);
      if (destination[0] == '/') destination = destination.slice(1);
      destination = '#' + destination;

      html += "<a href='" + destination + "'>" + url + "</a>";

      var match = build_text_matches_html(matches[i].text_matches);
      html += match;
    }

  }
  html += "</ul>";

  return html;
}

function display_search_results(data) {
  var results_html = "<h1>Search Results</h1>";

  if (data.items.length) {
    hide_error();
    results_html += build_result_matches_html(data.items);
  } else {
    show_error("Oops! No matches found");
  }

  $(ditto.content_id).html(results_html);
}

// Example input/output
//  trim_start_with_end("aabccd", "foo/aab") -> "ccd"
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

function github_search(query) {
  // build github search api url string
  var github_api = "https://api.github.com/";
  var search = "search/code?q=";
  var github_repo = ditto.github_username + "/" + ditto.github_repo;
  var search_details = "+in:file+language:markdown+repo:";

  var url = github_api + search + query + search_details + github_repo;
  var accept_header = "application/vnd.github.v3.text-match+json";

  $.ajax(url, { headers: { Accept: accept_header } }).done(function (data) {
    display_search_results(data);
  });
}

function searchbar_listener(event) {
  // when user presses ENTER in search bar
  if (event.which === 13) {
    var q = $("input[name=" + ditto.search_name + "]").val();
    if (q !== "") {
      location.hash = "#search=" + q;
    }
  }
}

function replace_symbols(text) {
  // replace symbols with underscore
  return text.replace(/[&\/\\#,+=()$~%.'":*?<>{}\ \]\[]/g, "_").toLowerCase();
}

function li_create_linkage(li_tag, header_level) {
  // add custom id and class attributes
  html_safe_tag = replace_symbols(li_tag.text());
  li_tag.attr("id", html_safe_tag);
  //li_tag.attr("class", "link");

  make_link(li_tag);

  // add click listener - on click scroll to relevant header section
  $(ditto.content_id + " li#" + li_tag.attr("id")).click(function (event) {
    event.preventDefault();

    // scroll to relevant section
    var header = find_header_at_level(li_tag.attr("id"), header_level);
    scroll_to_header(header, /*animate:*/true);
  });
}

function find_header(anchor_name) {
  // anchor name, is, ie, "my_custom_title" which has been created from "My custom title"
  for (var i = 1; i <= 6; i++) {

    var header = find_header_at_level(anchor_name, i);
    if (header.length == 0) continue;

    return header;
  }
}

function find_header_at_level(anchor_name, level) {
  return $(ditto.content_id + " h" + level + "." + anchor_name);
}

function scroll_to_header(header, animate) {
  $('html, body').animate({
    scrollTop: header.offset().top
  }, animate ? 200 : 0);

  // highlight the relevant section
  header.css('color', "#4682BE");
  header.css('text-decoration', 'underline');
}

function make_link(element) {
  var title = $(element).text();
  var anchor_name = replace_symbols(title);
  var link = window.location.href.split('?')[0] + "?" + anchor_name;

  $(element).text("");
  $(element).append("<a href=" + link + ">" + title + "</a>");
}

function create_page_anchors() {
  // create page anchors by matching li's to headers
  // if there is a match, create click listeners
  // and scroll to relevant sections

  for (var i = 1; i <= 6; i++) {
    // parse all headers
    var headers = [];
    $(ditto.content_id + ' h' + i).map(function () {
      headers.push($(this).text());

      var id = replace_symbols($(this).text());
      $(this).addClass(id);

      make_link(this);
      $(this).children().css("color", "inherit");
    });

    // parse and set links between li and h2
    $(ditto.content_id + ' ul li').map(function () {
      for (var j = 0; j < headers.length; j++) {
        if (headers[j] === $(this).text()) {
          li_create_linkage($(this), i);
        }
      }
    });
  }
}

function normalize_paths() {
  // images
  $(ditto.content_id + " img").map(function () {
    var src = $(this).attr("src").replace("./", "");
    if ($(this).attr("src").slice(0, 5) !== "http") {
      var url = location.hash.replace("#", "");

      // split and extract base dir
      url = url.split("/");
      var base_dir = url.slice(0, url.length - 1).join("/");

      // normalize the path (i.e. make it absolute)
      if (base_dir) {
        $(this).attr("src", base_dir + "/" + src);
      } else {
        $(this).attr("src", src);
      }
    }
  });
}

function show_error(err_msg) {
  $(ditto.error_id).html(err_msg);
  $(ditto.error_id).show();
}

function hide_error() {
  $(ditto.error_id).hide();
}

function show_loading() {
  $(ditto.loading_id).show();
  $(ditto.content_id).html("");  // clear content

  // infinite loop until clearInterval() is called on loading
  ditto.loading = setInterval(function () {
    $(ditto.loading_id).fadeIn(1000).fadeOut(1000);
  }, 2000);
}

function hide_loading() {
  clearInterval(ditto.loading);
  delete ditto.loading;
  $(ditto.loading_id).hide();
}

function escape_github_badges(data) {
  $("img").map(function () {
    var ignore_list = [
      "travis-ci.org",
      "coveralls.io"
    ];
    var src = $(this).attr("src");

    var base_url = src.split("/");
    var protocol = base_url[0];
    var host = base_url[2];

    if ($.inArray(host, ignore_list) >= 0) {
      $(this).attr("class", "github_badges");
    }
  });
  return data;
}

function get_file(path, processor, failed, always) {
  var CACHE_EXPIRATION_MINUTES = 5;
  lscache.enableWarnings(true);

  var data = lscache.get(path);
  if (data) {
    console.log("found cached result for " + path);
    processor(data);

    if (always)
      always();
  }

  else {
    console.log("running query for " + path);
    $.get(path, function (data) {
      lscache.set(path, data, CACHE_EXPIRATION_MINUTES);
      processor(data);
    }).fail(failed)
      .always(always)
  }
}

function page_getter() {
  // When we fetch a new page we want to scroll back to the
  // top of the window, otherwise we may show the user the
  // middle of an existing document.
  window.scrollTo(0, 0);

  var path = location.hash.replace("#", "./");
  var target = null;

  var idx = path.indexOf("?");
  if (idx >= 0) {
    target = path.substring(idx + 1);
    path = path.substring(0, idx);
  }

  // default page if hash is empty
  var current_page = location.pathname.split("/").pop();
  if (current_page === "index.html") {
    path = location.pathname.replace("index.html", ditto.index);
    normalize_paths();

  } else if (path === "") {
    path = window.location + ditto.index;
    normalize_paths();

  } else {
    path = path + ".md";
  }

  // otherwise get the markdown and render it
  hide_error();
  show_loading();

  get_file(path,
    /*processor:*/ function (data) {
      // compile the data
      data = marked(data);
      $(ditto.content_id).html(data);

      escape_github_badges(data);
      normalize_paths();
      create_page_anchors();

      $('pre code').each(function (i, block) {
        if (typeof hljs !== "undefined") {
          hljs.highlightBlock(block);
        }
      });

      // move to target
      if (target) {
        var header = find_header(target);
        if (header)
          scroll_to_header(header, /*animate:*/false);
      }
    },
    /*error:*/ function () {
      show_error("Opps! ... File not found!");
    },
    /*always:*/ function () {
      hide_loading();
    });

  // hide loading after five seconds... sometimes the *always* function is not invoked
  setTimeout(hide_loading, 5000);
}

function router() {
  var hash = location.hash;

  if (hash.slice(1, 7) !== "search") {
    page_getter();
  } else {
    github_search(hash.replace("#search=", ""));
  }
}

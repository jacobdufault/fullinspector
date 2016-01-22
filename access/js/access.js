"use strict";

$(document).ready(function() {
  var textInput = $('#submit-form input.text');
  var buttonInput = $('#submit-form input.button');
  var loadingDiv = $('#submit-form div.sk-cube-grid');
  var errorMsg = $('#submit-form p.error');
  var downloads = $('#downloadContainer');

  function setLoaderVisible(is_visible) {
    if (is_visible) {
      buttonInput.fadeOut(function() {
        loadingDiv.fadeIn();
      });
    }
    else {
      loadingDiv.fadeOut(function() {
        buttonInput.fadeIn();
      });
    }
  }

  function onAccessRequestCompleted(is_allowed, results) {
    setLoaderVisible(false);

    if (is_allowed == false) {
      textInput.attr('style', "border-radius: 5px; border:#FF0000 1px solid;");
      errorMsg.fadeIn();
      return;
    }

    downloads.fadeIn();
    // TODO: Populate downloads.
  }

  buttonInput[0].addEventListener('click', function() {
    console.log('got value ' + textInput[0].value);
    setLoaderVisible(true);

    setTimeout(function() {
      onAccessRequestCompleted(true, []);
    }, 200);
  });
});
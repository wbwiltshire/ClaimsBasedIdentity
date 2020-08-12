//Ajax library of common functions

function ajaxGet(options) {
     //Make the call un-cached
     $.ajax({
          url: options.url,
          cache: false,
          type: 'GET',
          success: function (result) {
               options.success(result);
          },
          error: function (result) {
               displayError(result);
          }   
     });
}

function ajaxPost(options) {
     $.ajax({
          url: options.url,
          type: 'POST',
          data: options.data,
          dataType: options.dataType,
          contentType: options.contentType,
          success: function (result, status, xhr) {
               options.success(result, status, xhr);
          },
          error: function (result, status, xhr) {
               // displayError(result, status);
               options.error(result, status, xhr);
          }
     });
}

function ajaxPut(options) {
     $.ajax({
          url: options.url,
          type: 'PUT',
          data: options.data,
          dataType: options.dataType,
          contentType: options.contentType,
          success: function (result, status, xhr) {
               options.success(result, status, xhr);
          },
          error: function (result, status, xhr) {
               // displayError(result, status);
               options.error(result, status, xhr);
          }
     });
}

function ajaxDelete(options) {
     $.ajax({
          url: options.url,
          type: 'DELETE',
          success: function (result, status, xhr) {
               options.success(result, status, xhr);
          },
          error: function (result, status, xhr) {
               // displayError(result, status);
               options.error(result, status, xhr);
          }
     });
}

function displayError(result) {
     $('#errorDisplay').css('visibility', 'visible');
     $('#errorDisplay').show();
     $('#errorDisplay').html(result.responseText);
};
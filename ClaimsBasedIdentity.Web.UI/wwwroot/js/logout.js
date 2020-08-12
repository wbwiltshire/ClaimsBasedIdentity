document.addEventListener("DOMContentLoaded", function (event) {

     // Hook the Logout element
     $('#logout').click(function () {
          submitLogout();
     });

     function submitLogout() {
          var data = new FormData();
          var xhr = new XMLHttpRequest();
          xhr.open("POST", "/Account/Logout");
          xhr.responseType = 'json';
          xhr.onload = function () {
               //console.log(this.response, this.status);

               // Redirect to url
               window.location.href = this.response.url;
          };
          xhr.send(data);
          return false;       // Prevent HTML FORM submit
     }
});
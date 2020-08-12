document.addEventListener("DOMContentLoaded", function (event) {

     var parms = (new URL(location)).searchParams;

     var action = parms.get('action');

     if (action === null || action.toUpperCase() === 'LOGIN') {
          // Show the login modal
          var login = document.getElementById('#loginModal');
          $("#loginModal").modal();
     }
     else {
          // Show the register modal
          var login = document.getElementById('#registerModal');
          $("#registerModal").modal();
     }

     // Toggle the modals off, when we switch
     $("#registerLink").click(function () {
          $("#loginModal").modal('hide');
     });
     $("#loginLink").click(function () {
          $("#registerModal").modal('hide');
     });

     // Hook the Register PW inputs
     $('.registerPW').change(function () {
          confirmPW();
     });

     // To confirm the password, the following must be true:
     // 1) the confirm and regular password must match exactly
     // 2) the password must meet all of the password policies (e.g. length, complexity, etc.)
     function confirmPW() {
          const password = document.getElementById('RegisterPassword');
          const confirm = document.getElementById('ConfirmRegisterPassword');
          const passwordMin = 6;
          const passwordMax = 100;
          var fail = false;

          // Password policies:
          // 1) Must contain a numeric digit: false
          // 2) Must contain a non-alpha character (i.e. special char): false
          // 3) Must contain an uppercase charcter: false
          // 4) Must contain a lowercase character: false
          // 5) Length must be a minimum of x characters: 6
          // 6) Length must be 100 characters or less: true
          if (password.value.length < passwordMin || password.value.length > passwordMax) {
               password.setCustomValidity('The password must be at least ' + passwordMin.toString() + ' and at maximum of ' + passwordMax.toString() + ' characters long.');
               fail = true;
          }

          if (!fail && !hasDigit(password.value)) {
               password.setCustomValidity('The password must a numeric digit.');
               fail = true;
          }

          if (!fail && !hasSpecialChar(password.value)) {
               password.setCustomValidity('The password must a special character.');
               fail = true;
          }

          if (!fail && !hasLowerCase(password.value)) {
               password.setCustomValidity('The password must a lowercase character.');
               fail = true;
          }

          if (!fail && !hasUpperCase(password.value)) {
               password.setCustomValidity('The password must an uppercase character.');
               fail = true;
          }

          // If no prior problems, then check if the passwords match
          if (!fail) {
               password.setCustomValidity('');
               if (confirm.value === password.value)
                    confirm.setCustomValidity('');
               else
                    confirm.setCustomValidity('Passwords do not match');
          }
     }

     function hasDigit(str) {
          var regex = /\d/;
          return (regex.test(str));
     }

     function hasSpecialChar(str) {
          var regex = /^[a-zA-Z0-9!@#\$%\^\&*\)\(+=._-]+$/g;
          return (regex.test(str));
     }

     function hasUpperCase(str) {
          var regex = /[A-Z]/;
          return (regex.test(str));
     }

     function hasLowerCase(str) {
          var regex = /[a-z]/;
          return (regex.test(str));
     }
});
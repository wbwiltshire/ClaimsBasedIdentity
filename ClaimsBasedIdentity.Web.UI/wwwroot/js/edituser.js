$(document).ready(function () {
     var emailConfirmed = document.getElementById('emailConfirmed');
     var phoneConfirmed = document.getElementById('phoneConfirmed');
     var twoFactor = document.getElementById('twoFactor');

     // Hook the deletes for all of the roleLinks
     addDeleteRoleHooks(document);

     $('#add').click(function () {
          var role = document.getElementById("selectRole");
          AddRole(role.value, role.options[role.selectedIndex].text);
     });

     updateEMailConfirmedDisplay($('#emailconfirmed'));
     updatePhoneConfirmedDisplay($('#phoneconfirmed'));
     updateTwoFactorDisplay($('#twofactor'));

     $('#emailconfirmed').click(function (e) {
          e.preventDefault();
          if (emailConfirmed.value === 'True') {
               emailConfirmed.value = 'False';
               updateEMailConfirmedDisplay($(this));
          }
          else {
               emailConfirmed.value = 'True';
               updateEMailConfirmedDisplay($(this));
          }
     });

     $('#phoneconfirmed').click(function (e) {
          e.preventDefault();
          if (phoneConfirmed.value === 'True') {
               phoneConfirmed.value = 'False';
               updatePhoneConfirmedDisplay($(this));
          }
          else {
               phoneConfirmed.value = 'True';
               updatePhoneConfirmedDisplay($(this));
          }
     });

     $('#twofactor').click(function (e) {
          e.preventDefault();
          if (twoFactor.value === 'True') {
               twoFactor.value = 'False';
               updateTwoFactorDisplay($(this));
          }
          else {
               twoFactor.value = 'True';
               updateTwoFactorDisplay($(this));
          }
     });

     function AddRole(value, text) {
          //alert("role: " + role);
          var newRoleId = value + '-' + text;
          var currentRole = document.getElementById(newRoleId);
          if (currentRole === null) {
               var roleList = document.getElementById("roles");
               var newRole = document.createElement("a");
               newRole.setAttribute('href', '#');
               newRole.onclick = function (el) {
                    DeleteRole(el.target.id);
               };
               var newRoleHeader = document.createElement("h4");
               var newRoleSpan = document.createElement("span");
               newRoleSpan.id = newRoleId;
               newRoleSpan.className = 'badge badge-pill badge-success';
               newRoleSpan.textContent = text;

               //chain them together
               newRoleHeader.appendChild(newRoleSpan);
               newRole.appendChild(newRoleHeader);
               roleList.appendChild(newRole);

               //update the RoleText
               var searchRoles = document.getElementsByName('User.RoleBadges')[0];
               searchRoles.value += newRoleId + '|';
          }
          // else this user already has the role
     }

     function addDeleteRoleHooks(root) {
          var roleLinks = root.getElementsByClassName('roleLinks');
          Array.prototype.forEach.call(roleLinks, function (el) {
               el.onclick = function (el) {
                    DeleteRole(el.target.id);
               };
          });
     }

     function DeleteRole(id) {
          // update the RoleText
          var roleTextString = id + '|';
          var roleText = document.getElementsByName('User.RoleBadges')[0];
          roleText.value = roleText.value.replace(roleTextString, '');

          // delete the role
          var role = document.getElementById(id);
          return role.parentNode.removeChild(role);
     }

     function updateEMailConfirmedDisplay(elem) {
          if (emailConfirmed.value === 'True') {
               elem.val('True');
               elem.removeClass('btn-danger');
               elem.addClass('btn-success');
          }
          else {
               elem.val('False');
               elem.removeClass('btn-success');
               elem.addClass('btn-danger');
          }
     };

     function updatePhoneConfirmedDisplay(elem) {
          if (phoneConfirmed.value === 'True') {
               elem.val('True');
               elem.removeClass('btn-danger');
               elem.addClass('btn-success');
          }
          else {
               elem.val('False');
               elem.removeClass('btn-success');
               elem.addClass('btn-danger');
          }
     };

     function updateTwoFactorDisplay(elem) {
          if (twoFactor.value === 'True') {
               elem.val('True');
               elem.removeClass('btn-danger');
               elem.addClass('btn-success');
          }
          else {
               elem.val('False');
               elem.removeClass('btn-success');
               elem.addClass('btn-danger');
          }
     };

});
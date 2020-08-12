//Common javascript functions

function formatCurrency(amount) {
     var formatted = 0.00;
     if (amount < 0) {
          formatted = (amount * -1).toFixed(2);;
          return '($' + formatted.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,') + ')';
     }
     else {
          formatted = amount.toFixed(2);
          return '$' + formatted.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,');
     }
}

function formatDate(format, datetime) {
     var DateFormats = {
          short: "M/DD/YYYY",
          business: "M/DD/YYYY h:mm A",
          long: "dddd MM/DD/YYYY HH:mm",
          tv: "MM/DD/YY HH:mm"
     };

     if (moment) {
          format = DateFormats[format] || format;
          return moment(datetime).format(format);
     }
     else {
          return datetime;
     }
}

function formatPercent(amount) {
     formatted = amount.toFixed(2);
     return formatted + '%';
}

function getCurrencyValue(value) {
     var amount = 0.00;

     if (value.indexOf('(') > -1)
          amount = parseFloat(value.replace('$', '').replace(',', '').replace('(', '').replace(')', ''), 10) * -1;
     else
          amount = parseFloat(value.replace('$', '').replace(',', ''), 10);

     return amount;
}

function formatDecimal(amount, precision) {
     if (isNaN(amount))
          amount = 0.00;
     formatted = amount.toFixed(precision);
     return formatted;
}

//forward to url from context menu
function forwardTo(url) {
     window.location = url;
}
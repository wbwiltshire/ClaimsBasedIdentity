	var blocks = {};
	var DateFormats = {
		short: "MM/DD/YYYY",
          business: "MM/DD/YYYY hh:mm A",
          long: "dddd MM/DD/YYYY HH:mm",
          sql: "YYYY-MM-DD HH:mm:00.000",
          tv: "MM/DD/YY HH:mm"
	};

    //Register Handlebars helpers
    Handlebars.registerHelper('block', function(name) {
	    var val = (blocks[name] || []).join('\n');

	    // clear the block
	    blocks[name] = [];
	    return val;
    });
	Handlebars.registerHelper('contentFor', function(name, options) {
          var block = blocks[name];
          if (!block) {
               block = blocks[name] = [];
          }
          block.push(options.fn(this));
	});
	Handlebars.registerHelper('ifCond', function (v1, v2, options) {
		if (v1 === v2) {
			return options.fn(this);
		}
		return options.inverse(this);
	});
	Handlebars.registerHelper('formatDate', function (format, datetime) {
          if (format === 'tv')
               datetime = datetime * 1000;
          if (moment) {
               format = DateFormats[format] || format;
               return moment(datetime).format(format);
          }
          else {
               return datetime;
          }
	});
	Handlebars.registerHelper("formatCurrency", function(amount) {
          if (amount < 0) {
               formatted = (amount * -1).toFixed(2);;
               return '($' + formatted.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,') + ')';
          }
          else {
               formatted = amount.toFixed(2);
               return '$' + formatted.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,');
          }
	});
     Handlebars.registerHelper("formatCurrencyCalc", function (operation, value1, value2) {
          var amount = 0.00;
          switch (operation) {
               case '+':
                    amount = value1 + value2;
                    break;
               case '-':
                    amount = value1 - value2;
                    break;
               case '*':
                    amount = value1 * value2;
                    break;
               case '/':
                    amount = value1 / value2;
                    break;
          }
          if (amount < 0) {
               formatted = (amount * -1).toFixed(2);;
               return '($' + formatted.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,') + ')';
          }
          else {
               formatted = amount.toFixed(2);
               return '$' + formatted.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,');
          }
     });
	Handlebars.registerHelper("formatPercent", function (amount) {
          amount = amount;
          formatted = amount.toFixed(2);
          return formatted + '%';
	});
     Handlebars.registerHelper("formatPercentCalc", function (operation, value1, value2) {
          var amount = 0.00;
          switch (operation) {
               case '+':
                    amount = (value1 + value2) / value1;
                    break;
               case '-':
                    amount = (value1 - value2) / value1;
                    break;
               case '*':
                    amount = value1 * value2;
                    break;
               case '/':
                    amount = value1 / value2;
                    break;          }
          formatted = amount.toFixed(2);
          return formatted + '%';
     });
     Handlebars.registerHelper("formatShares", function (value) {
          return sprintf('%0.4f', value);
     });
     Handlebars.registerHelper("formatFixed", function (prec, value) {
          var fixedAmount = parseFloat(value);
          var precision = parseInt(prec);
          return fixedAmount.toFixed(precision);
     });
     Handlebars.registerHelper("abbr", function (len, text) {
          var result = '';
          if (text !== null) {
               result = text.substring(0,len);
          }
          return result;
     });
     Handlebars.registerHelper("counter", function (index) {
     return index + 1;
     });

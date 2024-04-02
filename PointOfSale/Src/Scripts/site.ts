import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './../Styles/site.css';

$(document).ready(function () {
    //TODO Turn this into a real function that takes parms and creates the eval to bind electron to html objects
    (function () {
        try {
            eval("const { ipcRenderer } = require('electron'); ipcRenderer.on('item-scan', (_event, arg) => { $('#barcodeText').val(arg); $('#enterBarcodeButton').click(); });");
        } catch (err) {
            console.log('electron bridge not available.  need to enter upcs manually');
        }
    }());
});

// TODO: Quick and dirty way to format an input for money. Need to investigate this further.
$(document).ready(function () {
    $(function () {
        var input = ""; //holds current input as a string

        $("input.currency-input").keydown(function (e) {
            //handle backspace key
            if (e.keyCode == 13 && input.length > 0) {
                e.stopPropagation();
                $('#cashTenderButton').click();
            }
            else if (e.keyCode == 8 && input.length > 0) {
                input = input.slice(0, input.length - 1); //remove last digit
                $(this).val(formatNumber(input));
            }
            else {
                var key = getKeyValue(e.keyCode);
                if (key) {
                    input += key; //add actual digit to the input string
                    $(this).val(formatNumber(input)); //format input string and set the input box value to it
                }
            }
            return false;
        });

        function getKeyValue(keyCode) {
            if (keyCode > 57) { //also check for numpad keys
                keyCode -= 48;
            }
            if (keyCode >= 48 && keyCode <= 57) {
                return String.fromCharCode(keyCode);
            }
        }

        function formatNumber(input) {
            if (isNaN(parseFloat(input))) {
                return "0.00"; //if the input is invalid just set the value to 0.00
            }
            var num = parseFloat(input);
            return (num / 100).toFixed(2); //move the decimal up to places return a X.00 format
        }
    });
});
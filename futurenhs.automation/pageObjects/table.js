const helpers = require('../util/helpers');
const basePage = require('./basePage');

class table extends basePage{
    /**
     * Function to select the desired table from an object based on the input in the script
     * @param {string} tableName - name declared in the script, this is manipulated into a single string value to then match the name in the tables object
     * @returns - the xpath of the desired table to interact with in the function 
     */
    getTable(tableName){
        var desiredTable = tableName.toLowerCase().replace(' ', '');
        var tables = {
            groupmembers:$('//table[@id="group-table-members"]'),
            pendingmembers:$('//table[@id="group-table-pending-members"]'),
            groupfiles:$('//table[@id="group-table-files"]'),
            filedetails:$('//table[@id="group-table-file"]'),
        }
        var chosenTable = tables[desiredTable]
        return chosenTable
    }

    /**
     * Function to "build" a raw values object of a table that exists within the browser
     * @param {string} tableName - name of the desired table, used as object name with known xpath
     * @returns - the fully built table that exists within the browser
     * Example of structure
     * [
        [th, th, th]
        [tr, tr, tr]
        [tr, tr, tr]
     * ]
     */
    tableParser(tableName, mobile) {
        var table = this.getTable(tableName);
        helpers.waitForLoaded(table);
        var tableValues = [] 

        if(!mobile){
            var headerElements = table.$$('th');
            var tableHeaders = []
            headerElements.forEach(element => {
                var headerText = element.getText();
                tableHeaders.push(headerText);
            });
            tableValues.push(tableHeaders);
        }

        var rowElements = table.$$('tr'); 
        var tableRow = []
        rowElements.forEach(element => {
            var rowValues = element.$$('td');   
            rowValues.forEach(item => { 
                var rowText = item.getText();
                tableRow.push(rowText);
            })  
            tableRow = []
            tableValues.push(tableRow);  
        });
        tableValues.pop();
        // NHS Specific, when mobile is truthy, it will convert the parsed mobile table to cleanly match the output of the desktop version
        // This provides consistent output for testing across the other functions
        if(mobile){
            var mobileTable = [];
            tableValues.forEach(item => {
                var outputData = item.map(item => item.toString().split('\n'));
                outputData.forEach(item => mobileTable.push(item));
            });
            tableValues = mobileTable;
        }
        return tableValues
    }

    /**
     * Simple validation check to ensure table exists within a page
     * @param {string} tableName - name of the desired table, used as object name with known xpath 
     */
    tableIsExisting(tableName){
        var table = this.getTable(tableName);
        try {
            helpers.waitForLoaded(table); 
        } catch (error) {
            throw new Error (`Could not find "${tableName}" table : ${error}`);
        }
    }

    /**
     * Function to validate a complete table (rows and columns), using the table parser to compare against the expected table
     * @param {string} tableName - name of the desired table, used as object name with known xpath 
     * @param {Array} expectedTable - Array of the expected table, to validate against the parser
     */
    tableValidation(tableName, expectedTable, mobile){
        var actualTable = this.tableParser(tableName, mobile);
        expectedTable.forEach((expectedRow, rowIndex) => {
            expectedRow.forEach((expectedCell, cellIndex) => {
                if(expectedCell === '[PrettyDate]') {                    
                    try {
                        expect(this.dateValidator(actualTable[rowIndex][cellIndex])).toEqual(true);
                    } catch (error) {
                        throw new Error(`Unable to match '${actualTable[rowIndex][cellIndex]}' to any known Pretty Date value`);
                    }
                } else if(expectedCell === ''){
                    return
                } else {
                    expect(expectedCell).toEqual(actualTable[rowIndex][cellIndex]);
                }
            });
        });
    }

    /**
     * Basic row validation on a table, finds and validates specific textual value within a table row
     * @param {string} rowValue - known value within the desired row, to help locate the row required.
     * @param {string} tableName - name of the desired table, used as object name with known xpath
     */
    tableRowExists(rowValue, tableName){
        var table = this.getTable(tableName);
        helpers.waitForLoaded(table);
        var tableRow = table.$(`./tbody/tr[*[contains(normalize-space(.), "${rowValue}")]]`);
        expect(tableRow.isExisting()).toEqual(true);
    }

    /**
     * Function to click a link within a row, by finding the row using a "rowKey" to check through the tableparser
     * @param {string} linkText - known textual value of the desired link to click
     * @param {string} rowKey - Key value used to determine the required row.
     * @param {string} tableName - name of the desired table, used as object name with known xpath
     */
    tableLinkClick(linkText, rowKey, tableName){
        var tableRow
        var table = this.tableParser(tableName);
        table.shift();
        for(var i = 0; i <= table.length; i++){
            if(table[i].includes(rowKey) === true){
                tableRow = i+1
                break;
            }
        }
        var tablePath = this.getTable(tableName);
        if(linkText === 'Edit') {
            var link = tablePath.$(`./tbody/tr[${tableRow}]/td/a[@class="btn-mvc"]`);
        } else {
            var link = tablePath.$(`./tbody/tr[${tableRow}]/td//a[contains(normalize-space(.), "${linkText}")]`);
        }
        helpers.waitForLoaded(link)
        helpers.click(link);
    }

    /**
     * Function to count the number of rows found on the existing table
     * @param {string} tableName - name of the desired table, used as object name with known xpath
     * @param {string} rowCount - expected number of rows on the table
     */
    rowCounter(tableName, rowCount){
        var expectedRows = parseInt(rowCount);
        var table = this.tableParser(tableName);
        table.shift()
        var foundRows = table.length;
        expect(foundRows).toEqual(expectedRows);
    }

    /**
     * negative validation when expecting a table to not exist
     * @param {string} tableName - name of the desired table, used as object name with known xpath
     */
    tableNotExisting(tableName){
        var table = this.getTable(tableName);
        expect(table.isExisting()).toEqual(false);
    }
}
module.exports = new table();
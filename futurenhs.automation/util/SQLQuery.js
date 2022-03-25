const sql = require('mssql');
const fs = require('fs');
var results = null;
class SQLQuery{
constructor(connectionString){
    sql.connect(connectionString);
}
/**
 * This method is for querying the database via sql query files that are being passed over.
 * @param {*} sqlQueryFile - The SQL query that you want to execute within this named file.
 */
Query(sqlQueryFile){
    fs.readFile('../sqlQueries/'+sqlQueryFile, 'utf8', function(err, data) {
        if (err) throw err;
        console.log(data);
        results = sql.query(data);
        });
    }
}
module.exports = new SQLQuery();
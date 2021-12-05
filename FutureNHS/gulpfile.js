const
	childProcess = require('child_process'),
    path = require('path'),
	gulp = require('gulp');

var exec = require('child_process').exec;

gulp.task("activate",
    function (callback) {
        console.log('running vNext Api');
		exec(`Powershell.exe  -executionpolicy Unrestricted -file ${path.join(process.cwd(), 'Run.ps1')}`,
            function (err, stdout, stderr) {
                console.log(stderr);
                console.log(stdout);
                console.log(err);
                callback(err);
            });
    });


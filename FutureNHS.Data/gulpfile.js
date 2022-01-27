const { groupCollapsed } = require('console');
const gulp = require('gulp')
      childProcess = require('child_process');

/////////////////////////////////////
//  MSBUILD TASKS
/////////////////////////////////////
// Build .net solution
const msbuild = (done) => {

    process.env.PATH = `${process.env.PATH};C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\MSBuild\\Current\\Bin`;

    const proc = childProcess.spawn('msbuild.exe', [
        'FutureNHS.Data.sln',
        '-t:rebuild'
    ], {
        cwd: process.cwd()
    });

    const re = /SCS\d{4}/;
    proc.stdout.on('data', (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            return done(new Error('Security warning found when building project'));
        }
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error compiling project'));
        }

        return done();
    });
};

gulp.task(msbuild);

const msbuildAutomation = (done) => {

    process.env.PATH = `${process.env.PATH};C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\MSBuild\\Current\\Bin`;

    const proc = childProcess.spawn('msbuild.exe', [
        'FutureNHS.Data.sln',
        '-t:rebuild',
        '/p:Configuration=Automation'
    ], {
        cwd: process.cwd()
    });

    const re = /SCS\d{4}/;
    proc.stdout.on('data', (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            return done(new Error('Security warning found when building project'));
        }
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error compiling project'));
        }

        return done();
    });
};

gulp.task(msbuildAutomation);

///////////////////////////////////////
//  FutureNHS DB TASKS
//////////////////////////////////////

const deployFutureNHSDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\160\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Data.FutureNHS/bin/Debug/FutureNHS.Data.FutureNHS.dacpac',
        '/TargetDatabaseName:FutureNHS',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Data.FutureNHS/Report.xml',
        '/DeployScriptPath:./FutureNHS.Data.FutureNHS/Publish.sql',
        '/Profile:./FutureNHS.Data.FutureNHS/FutureNHS.Data.FutureNHS.publish.xml',
    ], {
        cwd: process.cwd()
    });

    sqlPackage.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stdout.on('close', () => {
        return done();
    })
};

gulp.task(deployFutureNHSDatabase); 

const deployAutomationFutureNHSDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\160\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Data.FutureNHS/bin/Automation/FutureNHS.Data.FutureNHS.dacpac',
        '/TargetDatabaseName:FutureNHS',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Data.FutureNHS/Report.xml',
        '/DeployScriptPath:./FutureNHS.Data.FutureNHS/Publish.sql',
        '/Profile:./FutureNHS.Data.FutureNHS/FutureNHS.Data.FutureNHS.publish.xml',
    ], {
        cwd: process.cwd()
    });

    sqlPackage.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stdout.on('close', () => {
        return done();
    });
};

gulp.task(deployAutomationFutureNHSDatabase); 


const dropFutureNHSDatabase = (done) => {
    var sqlCmd = childProcess.spawn('sqlcmd', [
        '-U',
        'sa',
        '-P',
        'password',
        '-Q',
        'DROP DATABASE FutureNHS',
    ], {
        cwd: process.cwd()
    });

    sqlCmd.stdout.on('data', (data) => {
        console.log(data.toString())
    })

    sqlCmd.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error dropping database'));
        }

        return done();
    });
};

gulp.task(dropFutureNHSDatabase);

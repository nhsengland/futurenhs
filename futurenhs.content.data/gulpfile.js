const gulp = require('gulp')
      childProcess = require('child_process');

/////////////////////////////////////
//  MSBUILD TASKS
/////////////////////////////////////
// Build .net solution
const msbuild = (done) => {

    process.env.PATH = `${process.env.PATH};C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\MSBuild\\Current\\Bin`;

    const proc = childProcess.spawn('msbuild.exe', [
        'futurenhs.content.data\\FutureNHS.Content.Data.sln',
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

const msbuildAutomation = (done) => {

    process.env.PATH = `${process.env.PATH};C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\MSBuild\\Current\\Bin`;

    const proc = childProcess.spawn('msbuild.exe', [
        'futurenhs.content.data\\FutureNHS.Content.Data.sln',
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

///////////////////////////////////////
//  FutureNHS DB TASKS
//////////////////////////////////////

const deployFutureNHSContentDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\160\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Content.Data/FutureNHS.Content.Data/bin/Debug/FutureNHS.Content.Data.dacpac',
        '/TargetDatabaseName:futurenhscontentapi',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Content.Data/FutureNHS.Content.Data/Report.xml',
        '/DeployScriptPath:./FutureNHS.Content.Data/FutureNHS.Content.Data/Publish.sql',
        '/Profile:./FutureNHS.Content.Data/FutureNHS.Content.Data/FutureNHS.Content.Data.publish.xml',
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

const deployAutomationFutureNHSContentDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\160\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Content.Data/FutureNHS.Content.Data/bin/Debug/FutureNHS.Content.Data.dacpac',
        '/TargetDatabaseName:futurenhscontentapi',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Content.Data/FutureNHS.Content.Data/Report.xml',
        '/DeployScriptPath:./FutureNHS.Content.Data/FutureNHS.Content.Data/Publish.sql',
        '/Profile:./FutureNHS.Content.Data/FutureNHS.Content.Data/FutureNHS.Content.Data.publish.xml',
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


const dropFutureNHSContentDatabase = (done) => {
    var sqlCmd = childProcess.spawn('sqlcmd', [
        '-U',
        'sa',
        '-P',
        'password',
        '-Q',
        'DROP DATABASE futurenhscontentapi',
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


module.exports = {
    msbuild,
    msbuildAutomation,
    deployFutureNHSContentDatabase,
    deployAutomationFutureNHSContentDatabase
}
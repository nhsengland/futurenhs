const gulp = require('gulp')
      childProcess = require('child_process');


/////////////////////////////////////
//  DOTNET CLEAN TASKS
/////////////////////////////////////
// Clean .net solution
const clean = (done) => {

    process.env.PATH = `${process.env.PATH}`;

    const proc = childProcess.spawn('dotnet', [
        'clean',
        'futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/FutureNHS.Content.Data.sqlproj',
        '/p:NetCoreBuild=true',
        '/p:SystemDacpacsLocation="futurenhs.content.api/FutureNHS.Content.Data/ExperimentalBuildTools"'
    ], {
        cwd: process.cwd()
    });

    const re = /SCS\d{4}/;
    proc.stdout.on('data', (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            return done(new Error('Security warning found when cleaning project'));
        }
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error cleaning project'));
        }

        return done();
    });
};


/////////////////////////////////////
//  DOTNET BUILD TASKS
/////////////////////////////////////
// Build .net solution
const dotnetBuild = (done) => {

    process.env.PATH = `${process.env.PATH}`;

    const proc = childProcess.spawn('dotnet', [
        'build',
        'futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/FutureNHS.Content.Data.sqlproj',
        '/p:NetCoreBuild=true',
        '/p:SystemDacpacsLocation="futurenhs.content.api/FutureNHS.Content.Data/ExperimentalBuildTools"'
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

// Build task - runs all the build tasks
const build = gulp.series(clean, dotnetBuild);

///////////////////////////////////////
//  FutureNHS DB TASKS
//////////////////////////////////////

const deployFutureNHSContentDatabase = (done) => {
    process.env.PATH = `${process.env.PATH}`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/bin/Debug/FutureNHS.Content.Data.dacpac',
        '/TargetDatabaseName:futurenhscontentapi',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:9um#Qu&6d3t5',
        '/DeployReportPath:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/Report.xml',
        '/DeployScriptPath:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/Publish.sql',
        '/Profile:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/FutureNHS.Content.Data.publish.xml',
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
    process.env.PATH = `${process.env.PATH}`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/bin/Debug/FutureNHS.Content.Data.dacpac',
        '/TargetDatabaseName:futurenhscontentapi',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:9um#Qu&6d3t5',
        '/DeployReportPath:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/Report.xml',
        '/DeployScriptPath:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/Publish.sql',
        '/Profile:futurenhs.content.api/FutureNHS.Content.Data/FutureNHS.Content.Data/FutureNHS.Content.Data.publish.xml',
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
        '9um#Qu&6d3t5',
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
    build,
    deployFutureNHSContentDatabase,
    deployAutomationFutureNHSContentDatabase
}

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
//  MvcForum DB TASKS
//////////////////////////////////////

const deployMvcForumDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\150\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Data.MvcForum/bin/Debug/FutureNHS.Data.MvcForum.dacpac',
        '/TargetDatabaseName:MvcForum',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Data.MvcForum/Report.xml',
        '/DeployScriptPath:./FutureNHS.Data.MvcForum/Publish.sql',
        '/Profile:./FutureNHS.Data.MvcForum/FutureNHS.Data.MvcForum.publish.xml',
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

gulp.task(deployMvcForumDatabase); 

const deployAutomationMvcForumDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\150\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Data.MvcForum/bin/Automation/FutureNHS.Data.MvcForum.dacpac',
        '/TargetDatabaseName:MvcForum',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Data.MvcForum/Report.xml',
        '/DeployScriptPath:./FutureNHS.Data.MvcForum/Publish.sql',
        '/Profile:./FutureNHS.Data.MvcForum/FutureNHS.Data.MvcForum.publish.xml',
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

gulp.task(deployAutomationMvcForumDatabase); 


const dropMvcForumDatabase = (done) => {
    var sqlCmd = childProcess.spawn('sqlcmd', [
        '-U',
        'sa',
        '-P',
        'password',
        '-Q',
        'DROP DATABASE MvcForum',
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

gulp.task(dropMvcForumDatabase);

/////////////////////////////
//  IDENTITY DB TASKS
/////////////////////////////

const deployIdentityDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\150\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Data.Identity/bin/Debug/FutureNHS.Data.Identity.dacpac',
        '/TargetDatabaseName:FutureNHS_Identity',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Data.Identity/Report.xml',
        '/DeployScriptPath:./FutureNHS.Data.Identity/Publish.sql',
    ], {
        cwd: process.cwd()
    });

    sqlPackage.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stderr.on('error', (data) => {
        console.log(data.toString());
    });

    sqlPackage.stdout.on('close', () => {
        return done();
    })
};

gulp.task(deployIdentityDatabase); 

const deployAutomationIdentityDatabase = (done) => {
    process.env.PATH = `${process.env.PATH};C:\\Program Files\\Microsoft SQL Server\\150\\DAC\\bin`;

    var sqlPackage = childProcess.spawn('sqlpackage', [
        '/Action:Publish',
        '/SourceFile:./FutureNHS.Data.Identity/bin/Automation/FutureNHS.Data.Identity.dacpac',
        '/TargetDatabaseName:FutureNHS_Identity',
        '/TargetServerName:localhost',
        '/TargetUser:sa',
        '/TargetPassword:password',
        '/DeployReportPath:./FutureNHS.Data.Identity/Report.xml',
        '/DeployScriptPath:./FutureNHS.Data.Identity/Publish.sql',
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

gulp.task(deployAutomationIdentityDatabase);


const dropIdentityDatabase = (done) => {
    var sqlCmd = childProcess.spawn('sqlcmd', [
        '-U',
        'sa',
        '-P',
        'password',
        '-Q',
        'DROP DATABASE FutureNHS_Identity',
    ], {
        cwd: process.cwd()
    });

    sqlCmd.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('Error dropping database'));
        }

        return done();
    });
};

gulp.task(dropIdentityDatabase);

const automationDb = (done) => {
    gulp.series(msbuildAutomation, deployAutomationMvcForumDatabase, deployAutomationIdentityDatabase)();

    return done();
}

gulp.task(automationDb)

const buildDb = (done) => {
    gulp.series(msbuild, deployMvcForumDatabase, deployIdentityDatabase)();
    
    return done();
}

gulp.task(buildDb);
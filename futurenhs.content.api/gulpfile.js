const
	childProcess = require('child_process'),
    path = require('path'),
	gulp = require('gulp');

var exec = require('child_process').exec;

// Build .net solution
const msbuild = (done) => {

    process.env.PATH = `${process.env.PATH}`;

    const proc = childProcess.spawn('dotnet', [
        'build'
    ], {
        cwd: path.join(process.cwd(), 'futurenhs.content.api/Umbraco9ContentApi.Umbraco')
    });

    const re = /SCS\d{4}/;
    proc.stdout.on('data', (data) => {
        console.log(data.toString());

        const match = re.exec(data.toString());
        if (match) {
            done(new Error('Security warning found when building project'));
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

// Start the site
const startSite = (done) => {

    process.env.PATH = `${process.env.PATH}`;

    const proc = childProcess.spawn('node', [
        '../../node_modules/pm2/bin/pm2',
        'start',
        'dotnet',
        '--name=nhs.futures.content.api',
        '--',
        'run'
    ], {
        cwd: path.join(process.cwd(), 'futurenhs.content.api/Umbraco9ContentApi.Umbraco')
    });

    proc.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('None zero error code returned when attempting to start Umbraco9ContentApi.Umbraco'));
        }

        return done();
    });

};

// Stop the site
const stopSite = (done) => {

    const proc2 = childProcess.spawn('node', [
        './node_modules/pm2/bin/pm2',
        'delete',
        'nhs.futures.content.api'
    ], {
        cwd: process.cwd()
    });

    proc2.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    proc2.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc2.on('close', (code) => {
        return done();
    });

};

module.exports = {
    msbuild,
    startSite,
    stopSite
}


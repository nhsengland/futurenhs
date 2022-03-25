const
	childProcess = require('child_process'),
    path = require('path'),
	gulp = require('gulp');

const sequenceArgs = (list) => {
	return (done) => {
		const doNext = () => {
			if (list.length === 0) {
				return Promise.resolve();
			}

			const next = list.shift();

			return new Promise((resolve, reject) => {
				const proc = childProcess.spawn('node', [
					'../node_modules/gulp/bin/gulp.js'
				].concat(next), {
					cwd: process.cwd()
				});

				proc.stdout.on('data', (data) => {
					console.log(data.toString().replace(/\n$/, ''));
				});

				proc.stderr.on('data', (data) => {
					console.log(data.toString().replace(/\n$/, ''));
				});

				proc.on('close', (code) => {
					if (code !== 0) {
						reject(new Error('None zero error code returned'));
						process.exit(code);
						return;
					}

					return resolve();
				});
			}).then(doNext);
		};

		doNext().then(done).catch(done);
	};
}

const startApi = (done) => {


    const proc = childProcess.spawn('node', [
        '../node_modules/pm2/bin/pm2',
        'start',
		`dotnet run`,
		'--name=FutureNHS.Api'
   
    ], {
		cwd: path.join(process.cwd(), 'FutureNHS.Api')
    });

    proc.stdout.on('data', (data) => {
        console.log(data.toString());
    });

    proc.stderr.on('data', (data) => {
        console.log(data.toString());
    });

    proc.on('close', (code) => {
        if (code !== 0) {
            return done(new Error('None zero error code returned'));
        }

        return done();
    });
};
gulp.task(startApi);

gulp.task('start:service', (done) => {
	var name = process.argv[3].replace('--name=', '');

	const proc = childProcess.spawn('node', [
		'../node_modules/pm2/bin/pm2',
		'start',
		`dotnet run`,
		'--name=' + name,
	]);

	proc.stdout.on('data', (data) => {
		console.log(data.toString());
	});

	proc.stderr.on('data', (data) => {
		console.log(data.toString());
	});

	proc.on('close', (code) => {
		if (code !== 0) {
			return done(new Error('None zero error code returned'));
		}

		return done();
	});
});

gulp.task('stop:service', (done) => {
	const proc = childProcess.spawn('node', [
		'../node_modules/pm2/bin/pm2',
		'delete',
		process.argv[3].replace('--name=', '')
	], {
		cwd: process.cwd()
	});

	proc.stdout.on('data', (data) => {
		console.log(data.toString());
	});

	proc.stderr.on('data', (data) => {
		console.log(data.toString());
	});

	proc.on('close', (code) => {
		return done();
	});
});

gulp.task('nuget:restore', (done) => {
	const proc = childProcess.spawn('dotnet', [
		'restore',
        '--source',
		'https://api.nuget.org/v3/index.json',
	], {
		cwd: process.cwd()
	});

	proc.stdout.on('data', (data) => {
		console.log(data.toString());
	});

	proc.stderr.on('data', (data) => {
		console.log(data.toString());
	});

	proc.on('close', (code) => {
		return done();
	});
});

gulp.task('activate', (done) => {

	var result = sequenceArgs([
		['stop:service', '--name=ASPNETCORE_ENVIRONMENT=local dotnet FutureNHS.Api'],
		['nuget:restore'],
		['start:service', '--name=ASPNETCORE_ENVIRONMENT=local dotnet FutureNHS.Api'],
	]);

	return result(done);
});

gulp.task('deactivate', sequenceArgs([
	['stop:service', '--name=ASPNETCORE_ENVIRONMENT=local dotnet FutureNHS.Api'],
]));
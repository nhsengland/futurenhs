const { series, parallel } = require('gulp'),
    api = require('./futurenhs.api/gulpfile'),
    db = require('./futurenhs.api/FutureNHS.Data/gulpfile'),
	contentApi = require('./futurenhs.content.api/gulpfile'),
    contentDb = require('./futurenhs.content.api/FutureNHS.Content.Data/gulpfile'),
    app = require('./futurenhs.app/gulpfile');

/**
 * API TASKS
 */

 const activateApi = series(api.stopSite, api.msbuild, api.startSite);
 
 const activateContentApi = series(contentApi.stopSite, contentApi.msbuild, contentApi.startSite);

/**
 * DATABASE TASKS
 */

const activateDb = series(db.build, db.deployFutureNHSDatabase);

const activateAutomationDb = series(db.build, db.deployAutomationFutureNHSDatabase);

/**
 * CONTENT DATABASE TASKS
 */

 const activateContentDb = series(contentDb.build, contentDb.deployFutureNHSContentDatabase);

/**
 * APP TASKS
 */

const activateApp = series(app.stopSite, app.build, app.startSite);

// Watch task - runs all the web tasks then watches and re-runs tasks on subsequent changes
const watchApp = (done) => { 

    const watchers = () => {

        gulp.watch([`${uiPath}/images/**/*`], gulp.series(app.images));
        gulp.watch([`${uiPath}/icons/**/*`], gulp.series(app.icons));
        gulp.watch([`${uiPath}/favicon/**/*`], gulp.series(app.favicon));
        gulp.watch([`${uiPath}/fonts/**/*`], gulp.series(app.fonts));

    };

    series(app.build, watchers)();

    done();

};

/**
 * PLATFORM TASKS
 */
const acivatecontentdb = series(activateContentDb);

const activate = series(activateAutomationDb, activateContentDb, activateApi, activateContentApi, activateApp);

const activateNoApp = series(activateAutomationDb, activateContentDb, activateApi, activateContentApi);

const activateNoApi = series(activateAutomationDb, activateContentDb, activateApp, activateContentApi);

const activateNoAppApi = series(activateAutomationDb, activateContentDb, activateContentApi);

const activateNoUmbraco = series(activateAutomationDb, activateContentDb, activateApi, activateApp);

const activateNoUmbracoNoApi = series(activateAutomationDb, activateContentDb, activateApp);

const activateNoAutomation = series(activateDb, activateContentDb, activateApi, activateContentApi, activateApp);

const deactivate = series(api.stopSite, contentApi.stopSite, app.stopSite);

module.exports = {
    activate,
    activateNoApp,
    activateNoApi,
    activateNoAppApi,
    activateNoUmbraco,
    activateApi,
    activateContentApi,
    activateNoUmbracoNoApi,
    activateDb,
    activateAutomationDb,
    activateNoAutomation,
    activateContentDb,
    activateApp,
    deactivate,
    watchApp
}
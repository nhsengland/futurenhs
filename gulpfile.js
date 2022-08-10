const { series, parallel } = require('gulp'),
    mvcforum = require('./MVCForum/gulpfile'),
    db = require('./futurenhs.data/gulpfile'),
    api = require('./futurenhs.api/gulpfile'),
	contentApi = require('./futurenhs.content.api/gulpfile'),
    contentDb = require('./futurenhs.content.data/gulpfile'),
    app = require('./futurenhs.app/gulpfile');

    
/**
 * MVCFORUM TASKS
 */

const activateMvcForum = series(mvcforum.stopSite, mvcforum.msbuild, mvcforum.buildWeb, mvcforum.startSite);

/*const activateLight = (done) => {
    return gulp.series(mvcforum.stopSite, mvcforum.build, mvcforum.buildWebLight, mvcforum.startSite)();
};

const deactivate = (done) => {
    return gulp.series(mvcforum.stopSite)();
};
*/

/**
 * API TASKS
 */

 const activateApi = series(api.stopSite, api.msbuild, api.startSite);
 
 const activateContentApi = series(contentApi.stopSite, contentApi.msbuild, contentApi.startSite);

/**
 * DATABASE TASKS
 */

const activateDb = series(db.msbuild, db.deployFutureNHSDatabase);

const activateAutomationDb = series(db.msbuild, db.deployAutomationFutureNHSDatabase);

/**
 * CONTENT DATABASE TASKS
 */

 const activateContentDb = series(contentDb.msbuild, contentDb.deployFutureNHSContentDatabase);

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

const activate = series(activateAutomationDb, activateContentDb, activateMvcForum, activateApi, activateContentApi, activateApp);

const activateNoApp = series(activateAutomationDb, activateContentDb, activateMvcForum, activateApi, activateContentApi);

const activateNoApi = series(activateAutomationDb, activateContentDb, activateMvcForum, activateApp, activateContentApi);

const activateNoUmbraco = series(activateAutomationDb, activateContentDb, activateMvcForum, activateApi, activateApp);

const activateNoUmbracoNoApi = series(activateAutomationDb, activateContentDb, activateMvcForum, activateApp);

const activateNoAutomation = series(activateDb, activateContentDb, activateMvcForum, activateApi, activateContentApi, activateApp);

const deactivate = series(mvcforum.stopSite, api.stopSite, contentApi.stopSite, app.stopSite);

module.exports = {
    activate,
    activateNoApp,
    activateNoApi,
	activateNoUmbraco,
    activateApi,
	activateContentApi,
    activateNoUmbracoNoApi,
    activateMvcForum,
    activateDb,
    activateAutomationDb,
    activateNoAutomation,
    activateContentDb,
    activateApp,
    deactivate,
    watchApp
}
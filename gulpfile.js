const { series, parallel } = require('gulp'),
    mvcforum = require('./MVCForum/gulpfile'),
    db = require('./FutureNHS.Data/gulpfile'),
    api = require('./FutureNHS/gulpfile'),
    app = require('./vNext.App/gulpfile');

    
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

/**
 * DATABASE TASKS
 */

const activateDb = series(db.msbuild, db.deployFutureNHSDatabase);

const buildAutomationDb = series(db.msbuildAutomation, db.deployAutomationFutureNHSDatabase);

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

const activate = series(activateDb, activateMvcForum, activateApi, activateApp);

const activateNoApp = series(activateDb, activateMvcForum, activateApi);

const activateNoApi = series(activateDb, activateMvcForum, activateApp);

const activateAutomation = series(buildAutomationDb, activateMvcForum, activateApi, activateApp);

const deactivate = series(mvcforum.stopSite, api.stopSite, app.stopSite);

module.exports = {
    activate,
    activateNoApp,
    activateNoApi,
    activateAutomation,
    activateApi,
    activateMvcForum,
    activateDb,
    buildAutomationDb,
    activateApp,
    deactivate,
    watchApp
}


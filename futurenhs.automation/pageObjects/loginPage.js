const helpers = require('../util/helpers');
const basePage = require('./basePage');
const genericPage = require('./genericPage');

const userTypes = { 
    admin:{
        username: process.env[`ADMIN_USERNAME`],
        password: process.env[`ADMIN_PASSWORD`]
    },
    groupadmin:{
        username: process.env[`GROUP_ADMIN_USERNAME`],
        password: process.env[`GROUP_ADMIN_PASSWORD`]
    },
    user:{
        username: process.env[`USER_USERNAME`],
        password: process.env[`USER_PASSWORD`]
    },
    visreguser:{
        username: process.env[`VISUAL_REGRESSION_USERNAME`],
        password: process.env[`VISUAL_REGRESSION_PASSWORD`]
    },
    edituser:{
        username: process.env[`EDITABLE_USER_USERNAME`],
        password: process.env[`EDITABLE_USER_PASSWORD`]
    }
};

class loginPage extends basePage{
    /**
     * Function to login to the platform, keeping the journey within a function to keep the scripts clean. 
     * @param {string} desiredUser - user wanted to access the system, uses the name to find the login details in the "userTypes" object
     */
    loginUser(desiredUser){
        var user = userTypes[desiredUser.replace(/ /g, '')]
        var username = user.username
        var password = user.password
        helpers.waitForLoaded('//h1[text()[normalize-space() = "Log In"]]')
        helpers.waitForLoaded('//input[@id="UserName"]').setValue(username);
        helpers.waitForLoaded('//input[@id="Password"]').setValue(password);
        helpers.click('//button[@class="c-button c-button--min-width"]');
        helpers.waitForLoaded(`//div[@id="user-accordion"]`);
    }

    /**
     * Function to logout the user and validate the Log In page has loaded
     */
    logoutUser(){
        genericPage.selectAccordionItem('Log Off', 'usermenu')
        genericPage.selectDialogButton('confirm', 'logout');
        genericPage.contentValidation('header', 'Log In');
        genericPage.contentValidation('button', 'Log In');
    }

    /**
     * Function to logout the user and log in as a different one
     */
     switchUser(desiredUser){
        this.logoutUser()
        this.loginUser(desiredUser)
    }
}
module.exports = new loginPage();
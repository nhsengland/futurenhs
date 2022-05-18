const helpers = require('../util/helpers');
const basePage = require('./basePage');
const genericPage = require('./genericPage');

const userTypes = { 
    admin:{
        username: "autoAdmin@test.co.uk",
        password: "Tempest070",
        fullName: "Auto Admin"
    },
    groupadmin:{
        username: "autoGroupAdmin@test.co.uk",
        password: "Tempest070",
        fullName: "Auto GroupAdmin"
    },
    user:{
        username: "autoUser@test.co.uk",
        password: "Tempest070",
        fullName: "Auto User"
    },
    visreguser:{
        username: "VisRegUser@email.com",
        password: "Tempest101",
        fullName: "Vis Reg"
    },
    edituser:{
        username: "autoEditUser@test.co.uk",
        password: "Tempest101",
        fullName: "AutoEdit User"
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
        helpers.click('//summary[@class="c-accordion_toggle c-site-header-nav_root-nav-trigger"]');
        helpers.click('//a[@href="/log-out"]');
        genericPage.selectDialogButton('confirm', 'logout');
        this.loginUser(desiredUser)
    }
}
module.exports = new loginPage();
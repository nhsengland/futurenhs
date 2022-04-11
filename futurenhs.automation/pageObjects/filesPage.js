const helpers = require('../util/helpers');
const basePage = require('./basePage');
const fs = require('fs');
const crypto = require('crypto')
const PdfReader = require('pdfreader').PdfReader;
class filesPage extends basePage{
    /**
     * Function to upload a file that exists within the repository onto the system
     * @param {string} filePath - the known file path location of the desired file
     */
    mediaUpload(filePath) {
        var control = $(`//input[@type="file"]`);
        control.waitForExist();
        control.addValue(browser.uploadFile(process.cwd() + filePath));
    }

    /**
     * Function to create a temporary directory and then download a file into said directory
     * @param {string} fileType - file extension we expect and what we use to locate the file within the directory
     * @returns entire file path from Framework root to be then used in other functions
     */
    fileDownload(fileType, fileName){        
        if(browser === 'chrome'){
            var dir = fs.mkdtempSync(global.downloadDir + "/")
            browser.cdp('Page', 'setDownloadBehavior', {
                behavior: 'allow',
                downloadPath: dir,
            }); 
        } else {
            var dir = global.downloadDir
        }
        // click download button
        var tableRow = $(`//tbody/tr[*[contains(normalize-space(.), "${fileName}")]]`)
        helpers.waitForLoaded(tableRow);
        var downloadBtn = tableRow.$('./td/a[text()="Download file"]');
        helpers.click(downloadBtn);
        // locate file
        browser.waitUntil(()=> {return fs.readdirSync(dir).filter(item => item.endsWith(`.${fileType}`)).length > 0}, {timeout: 10000, timeoutMsg: "file not downloaded after 10s"});
        return dir + '/' + fs.readdirSync(dir).filter(item => item.endsWith(`.${fileType}`))[0]
    }

    /**
     * function to Check the content of a downloaded PDF against expected/known content
     * @param {Array} expectedContents - the known content we are wanting to check is within the downloaded PDF
     */
    pdfDownloadCheck(expectedContents){
        var dir = this.fileDownload(fileName.split('.')[1], fileName.split('.')[0]);
        // collate contents
        var actualPDFContent = "";
        var processing = true
        new PdfReader().parseFileItems(dir, function(err, item) {
            if (err) throw err;
            else if (err === undefined && item === undefined){processing = false} //undocumented, potential change risk
            else if (item.text) {actualPDFContent += " " + item.text}
        });
        browser.waitUntil(function() {return !processing})
        // validate contents of the downloaded PDF against expected values
        expect(actualPDFContent.includes(expectedContents.toString())).toEqual(true)
    }

    /**
     * Function to validate the hash is unchanged of a file that exists within /media, and has been uploaded and then downloaded by the system
     * @param {string} fileName - file we're checking against to locate the downloaded and original versions.
     */
    hashCompare(fileName){
        var dir = this.fileDownload(fileName.split('.')[1], fileName.split('.')[0]);
        function checksum(str) {
            return crypto
              .createHash('md5')
              .update(str, 'utf8')
              .digest('hex')
        }
        var downloadedFile = fs.readFileSync(dir);
        var oldFile = fs.readFileSync(`./media/${fileName}`);
        // compare hash values of both files
        expect(checksum(downloadedFile)).toEqual(checksum(oldFile));
    }
    
    /**
     * Validates the Collabora File Preview iframe is rendered on the page, and that it's set up to show a document. 
     * @param {*} mobile - returns a string of "mobile", used as truthy condition to check for mobile speciifc elements.
     */
    filePreviewExists(mobile){
        var collaboraFrame = $(`//iframe[@id="collabora-online-viewer"]`);
        collaboraFrame.waitForExist();
        browser.switchToFrame(collaboraFrame);
        var navMenu = $(`//nav/ul[@id="main-menu"]`);
        var documentContainer = $(`//div[@id="document-container"]`);
        helpers.waitForLoaded(navMenu);
        helpers.waitForLoaded(documentContainer);
        if(mobile){
            expect(documentContainer.getAttribute('class')).toEqual('readonly text-doctype slide-normal-mode');
        }
        var documentFrame = documentContainer.$(`//div/iframe[1]`);
        documentFrame.waitForExist();
        browser.switchToFrame(documentFrame);
        var document = $(`//html/body`);
        helpers.waitForLoaded(document);  
        browser.switchToParentFrame();
        browser.switchToParentFrame();
    }
    
    /**
     * Text compare validation of the displayed breadcrumbs in the browser
     * @param {string} expectedBreadcrumb - string variable of the expected textual value
     */
    breadcrumbValidation(expectedBreadcrumb){
        var firstBreadcrumb = expectedBreadcrumb.split(' > ')[0]
        var expectedBreadcrumb = expectedBreadcrumb.replace(/ > /g, '');
        var foundBreadcrumb = $(`//ol[@class="c-breadcrumb_list"][*[contains(normalize-space(.), "${firstBreadcrumb}")]]`);
        var breadcrumbText = foundBreadcrumb.getText().replace(/\.{3,}\n([^\n]*)/g, '...').replace(/\n/g, '');
        expect(breadcrumbText).toEqual(expectedBreadcrumb);
    }
    
    /**
     * 
     * @param {*} fileName 
     */
    imageUploadValidation(fileName){
        var fileUpload = $(`//input[@type="file"]`);
        var originalValue = fileUpload.getValue();
        expect(originalValue.includes(fileName)).toEqual(true)
    }
}
module.exports = new filesPage();
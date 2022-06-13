class Helpers{
  _resolveElement(element){
    if(typeof element === 'string'){
      var element = $(element)
    }
    return element
  }

  /**
  * safely clicks an item
  * @param {*} element - element or selector
  * @param {number} waitTime - the amount of time to wait in MS
  */
  click(element, waitTime = 10000){
    element = this._resolveElement(element)
    this.waitForLoaded(element, waitTime)
    element.scrollIntoView();
    element.click();
  }

  /**
   * waits for element to be loaded on the page ready for further interaction
   * @param {*} element - element or selector
   * @param {number} waitTime - the amount of time to wait in MS
   */ 
  waitForLoaded(element, waitTime = 10000){
    element = this._resolveElement(element);
    element.waitForEnabled({timeout:waitTime});
    element.waitForDisplayed({timeout:waitTime});
    return element
  }

  /**
   * generates a random GUID value as a string
   * @returns - string GUID value
   */
  randomIDGenerator(){
    var id = ''
    var idValues = 'abcdefghijklmnopqrstuvwxyz0123456789'
    var valuesLength = idValues.length
    for(var i = 0; i<32; i++){
      id += idValues.charAt(Math.floor(Math.random() * valuesLength));
    }
    var newID = id.slice(0, 8) + '-' + id.slice(9, 13) + '-' + id.slice(14, 18) + '-' + id.slice(19, 23) + '-' + id.slice(24, 32);
    return newID
  }

  /**
   * generates a random string to X no. of characters as defined in the param
   * @param {*} - desired length of string
   * @returns - string value
   */
  randomStringGenerator(length){
    var string = ''
    var stringVals = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'
    var stringLength = length.match(/[0-9]{1,}/g).toString();
    for(var i = 0; i<parseInt(stringLength); i++){
      string += stringVals.charAt(Math.floor(Math.random() * stringVals.length));
    }
    return string
  }

  /**
  *  in instances where more than one element is present but not enabled, allows you to quickly select only the active ones
  * @param {string} selector - a selector that $ or $$ would understand
  * @param {number} instance (optional) - in the case where you expect multible buttons, picks the specified one( if not provided gets first one )
  */
   getEnabledInstance(elementQuery, instance = 1 ){
    var result
    browser.waitUntil(()=>{
      try{
        result = $$(elementQuery)
      }
      catch (e){
        console.log(e)
        return false
      }
      result = result.filter((element => element.isClickable()));
      return result.length >= instance;
    },{timeout: 10000, timeoutMsg: `never found an enabled element for "${elementQuery}"`})
    return result[instance-1]
  }

  /**
   * helper to clear all textual content of a element, "CTRL + A, DEL"
   * @param {*} element - element that contains the content to be cleared
   */
   clearElement(element){        
    this.click(element)
    browser.keys(['Control', 'a'])
    browser.keys('Delete')
  }
}
module.exports = new Helpers()
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
  waitForLoaded(element, waitTime = 5000){
    element = this._resolveElement(element);
    element.waitForEnabled({timeout:waitTime});
    element.waitForDisplayed({timeout:waitTime});
    return element
  }

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
}
module.exports = new Helpers()
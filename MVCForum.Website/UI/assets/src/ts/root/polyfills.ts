import dialogPolyfill from '@modules/polyfills/dialog';

const dialogElements: Array<Element> = Array.from(document.getElementsByTagName('dialog'));

dialogElements.forEach( dialogElement => {

    dialogPolyfill.registerDialog(dialogElement);

});

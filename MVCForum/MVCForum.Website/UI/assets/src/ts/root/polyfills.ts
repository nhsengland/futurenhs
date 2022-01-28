import dialogPolyfill from "dialog-polyfill";


const dialogElement: any = document.querySelector('dialog');


    dialogPolyfill.registerDialog(dialogElement);


if (navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1) {
  const destination = document.getElementsByTagName('body')[0];
  // Declare a fragment:
  const fragment = document.createDocumentFragment();
  
  fragment.appendChild(dialogElement);

  destination.insertBefore(fragment, destination.firstChild);
}

if (typeof(dialogElement) != 'undefined' && dialogElement != null && typeof dialogElement.showModal !== "function") {
   
    const polyfill = document.createElement("script");
    polyfill.type = "text/javascript";
    polyfill.src = "@modules/dialog-polyfill/dist/dialog-polyfill.js";
    document.body.append(polyfill);
  
    const polyfillStyles = document.createElement("link");
    polyfillStyles.rel = "stylesheet";
    polyfillStyles.href = "@modules/dialog-polyfill/dialog-polyfill.css";
    document.head.append(polyfillStyles); 

  // Register polyfill on dialog element once the script has loaded
  dialogPolyfill.registerDialog(dialogElement);
}


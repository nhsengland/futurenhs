import dialogPolyfill from "dialog-polyfill";


const dialogElement = document.querySelector('dialog');


    dialogPolyfill.registerDialog(dialogElement);

    
if (typeof dialogElement.showModal !== "function") {
    // Load polyfill script
    const polyfill = document.createElement("script");
    polyfill.type = "text/javascript";
    polyfill.src = "./node_modules/dialog-polyfill/dist/dialog-polyfill.esm.js";
    document.body.append(polyfill);
  
    // Register polyfill on dialog element once the script has loaded
    polyfill.onload = () => {
      dialogPolyfill.registerDialog(dialogElement);
    };
  
    // Load polyfill CSS styles
    const polyfillStyles = document.createElement("link");
  
    polyfillStyles.rel = "stylesheet";
    polyfillStyles.href = "./node_modules/dialog-polyfill/dist/dialog-polyfill.css";
    document.head.append(polyfillStyles);
  }


export const lockBodyScroll: Function = (shouldLock: boolean) => {

    if(shouldLock){

        document.body.classList.add('u-overflow-hidden');

    } else {

        document.body.classList.remove('u-overflow-hidden');

    }


}
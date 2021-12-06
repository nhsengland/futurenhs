/**
 * A helper to push events to the GTM data layer
 */
export const pushgtmDataLayerEvent: Function = (eventName: string, id: string, meta?: any): void => {

    if(!eventName ){

        throw new Error('Push to GTM data layer missing required event name');

    }

    if(typeof id === 'undefined'){

        throw new Error('Push to GTM data layer missing required id');

    }

    window.dataLayer = (window as any).dataLayer || [];

    let payload: any = {
        'event': eventName
    };

    if(id){

        payload['id'] = id;

    }

    if(meta && typeof meta === 'object'){

        payload = Object.assign(payload, meta);

    }

    window.dataLayer.push(payload);

}
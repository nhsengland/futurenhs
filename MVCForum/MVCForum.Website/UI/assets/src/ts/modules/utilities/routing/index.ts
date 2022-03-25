/**
 * Get an object representation of a search params string
 */
export const getObjectFromSearchParams: Function = (search: string): object => {

    if(!search || typeof search !== 'string' || !search.length){

        return {}

    }

    const object: any = search
        .slice(1)
        .split('&')
        .map((pair: string) => pair.split('='))
        .reduce((obj: any, pair: any) => {

            const [key, value] = pair.map(decodeURIComponent);

            if(key){

                return ({ ...obj, [key]: value })

            }

        }, {}) || {};

    return object;

}

/**
 * Get a search params string from an object
 */
export const getSearchParamsFromObject: Function = (object: any): string => {

    let queryString: string = '';

    if(!object || typeof object !== 'object' || Object.keys(object).length === 0){

        return queryString;

    }

    Object.keys(object).forEach((key: string, index: number) => {

        const prefix: string = index === 0 ? '?' : '&';
        let value: any = object[key];

        if(!value){

            value = '';

        }

        queryString += `${prefix}${key}=${value}`;

    });

    return queryString;

}

/**
 * Ping an endpoint
 */
 export const ping: Function = (endPoint: string): void => {

    $.getJSON(endPoint);

}

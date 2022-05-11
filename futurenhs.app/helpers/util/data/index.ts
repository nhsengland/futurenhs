import { randomBytes } from 'crypto';

/**
 * Evaluates whether a property is defined and not null
 */
export const isDefined = (property: any): boolean => {
    return (
        typeof property !== 'undefined' && property !== null && property !== ''
    )
}

/**
 * Deletes an array item and returns a new array
 */
 export const deleteArrayItem = (array: Array<any>, fromIndex: number) => {

    const newArray: Array<any> = [...array];

    newArray.splice(fromIndex, 1);

    return newArray;

}

/**
 * Moves the index of an array item and returns a new array
 */
export const moveArrayItem = (array: Array<any>, fromIndex: number, toIndex: number) => {

    const newArray: Array<any> = [...array];
    const elementToMove: any = newArray[fromIndex];

    newArray.splice(fromIndex, 1);
    newArray.splice(toIndex, 0, elementToMove);

    return newArray;

}

/**
 * Creates a unique Id suitable for use in HTML
 */
export const createHtmlSafeId = (): string => randomBytes(6).toString('hex');

/**
 * Clones a serialisable object/array
 * NOT suitable for objects with non-serialisable data e.g. functions, Dates etc 
 */
export const simpleClone = (item) => JSON.parse(JSON.stringify(item));

/**
 * Returns whether an object has any own properties
 */
export const hasKeys = (object: Record<any, any>): boolean => {

    if(object) {
        return Object.keys(object).length > 0;
    }

    return false;

};

/**
 * Maps an object into a string
 */
export const getCsvStringFromObject = ({
    object,
    seperator,
}: {
    object: Record<string, string>
    seperator: string
}): string => {
    let string: string = ''

    Object.keys(object).forEach((name, index) => {
        string += `${name}=${object[name]}${
            index < Object.keys(object).length - 1 ? seperator : ''
        }`
    })

    return string
}

/**
 * Clears client service worker cache
 * Useful when non-GET requests will affect a subsequent GET request
 */
export const clearClientCaches = async (
    cacheNames: Array<string>
): Promise<void> => {
    return new Promise((resolve) => {
        if (typeof window !== 'undefined' && window.caches) {
            window.caches.keys().then((names) => {
                for (let name of names) {
                    if (cacheNames.includes(name)) {
                        caches.delete(name)
                    }
                }

                resolve()
            })
        } else {
            resolve()
        }
    })
}

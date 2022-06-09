import { Image } from "@appTypes/image"

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
    const newArray: Array<any> = [...array]

    newArray.splice(fromIndex, 1)

    return newArray
}

/**
 * Moves the index of an array item and returns a new array
 */
export const moveArrayItem = (
    array: Array<any>,
    fromIndex: number,
    toIndex: number
) => {
    const newArray: Array<any> = [...array]
    const elementToMove: any = newArray[fromIndex]

    newArray.splice(fromIndex, 1)
    newArray.splice(toIndex, 0, elementToMove)

    return newArray
}

/**
 * Clones a serialisable object/array
 * NOT suitable for objects with non-serialisable data e.g. functions, Dates etc
 */
export const simpleClone = (item) => JSON.parse(JSON.stringify(item))

/**
 * Returns whether an object has any own properties
 */
export const hasKeys = (object: Record<any, any>): boolean => {
    if (object) {
        return Object.keys(object).length > 0
    }

    return false
}

/**
 * Maps an object into a csv string
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
 * Maps a csv string into an object
 */
 export const getObjectfromCsvString = ({
    string,
    seperator,
}: {
    string: string,
    seperator: string,
}): Record<any, any> => {
    
    let object: Record<any, any> = {}

    const array: Array<string> = string.split(seperator);

    array.forEach((kvPair, index) => {
        const [key, value] = kvPair.split('=');
        object[key] = value;
    })

    return object
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

export const mapToProfileImageObject = (
    image: Record<string, any>,
    altText?: string
): Image => {
    if (!image) return null
    return {
        src: image.source,
        height: image.height,
        width: image.width,
        altText: altText ?? '',
    }
}

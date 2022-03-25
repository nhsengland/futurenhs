/**
 * Evaluates whether a property is defined and not null
 */
 export const isDefined = (property: any): boolean => {

    return typeof property !== 'undefined' && property !== null && property !== '';

}

/**
 * Maps an object into a string
 */
 export const getCsvStringFromObject = ({
    object,
    seperator
}: {
    object: Record<string, string>;
    seperator: string;
}): string => {
    
    let string: string = '';

    Object.keys(object).forEach((name, index) => {

        string += `${name}=${object[name]}${index < Object.keys(object).length -1 ? seperator : ''}`

    });

    return string;

};

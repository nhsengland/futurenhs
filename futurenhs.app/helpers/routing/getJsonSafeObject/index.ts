declare interface Config {
    object: Record<any, any>;
}

export const getJsonSafeObject = ({
    object
}: Config): Record<any, any> => {
        
    const safeObject: string = JSON.stringify(object, (key: any, value: any) => typeof value === 'undefined' ? null : value);

    return JSON.parse(safeObject);

};

export interface User {
    id: string;
    text: {
        userName: string;
    };
    image?: {
        source: string;
        altText: string;
    };    
}

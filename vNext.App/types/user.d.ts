export interface User {
    id: string;
    text: {
        userName: string;
        initials: string;
    };
    image: {
        source: string;
        altText: string;
    };    
}

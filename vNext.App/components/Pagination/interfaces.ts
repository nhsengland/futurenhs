export interface Props {
    content?: {
        descriptionHtml: string;    
    };
    navMenuList: Array<{
        url: string;
        text: string;
    }>;
}
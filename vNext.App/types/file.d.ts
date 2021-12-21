import { Image } from './image'; 

export interface File {
    type: string;
    name: string;
    bodyHtml: string;
    modified: string;
    modifiedBy: string;
    createdBy: string;
}

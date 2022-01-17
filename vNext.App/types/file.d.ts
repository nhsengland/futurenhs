import { Image } from './image'; 
import { BreadCrumbList } from './routing'; 

export interface Folder {
    id: string;
    type: 'folder' | 'file';
    name: string;
    bodyHtml?: string;
    modified?: string;
    path?: BreadCrumbList;
}

export interface File {
    id: string;
    type: 'folder' | 'file';
    name: string;
    bodyHtml: string;
    modified: string;
    modifiedBy: string;
    createdBy: string;
}

import { Image } from './image'; 
import { BreadCrumbList } from './routing'; 

export interface Folder {
    id: string;
    type: 'folder' | 'file';
    text: {
        name: string;
        body?: string;
    };
    modified?: string;
    path?: BreadCrumbList;
}

export interface FolderContent {
    id: string;
    type: 'folder' | 'file';
    extension?: string;
    name: string;
    modified?: string;
    modifiedBy?: {
        id: string;
        name: string;
    };
    createdBy?: {
        id: string;
        name: string;
    };
    text?: {
        body?: string;
    };
    path?: BreadCrumbList;
}

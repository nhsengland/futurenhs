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
    modifiedBy?: string;
    createdBy?: string;
    text?: {
        body?: string;
    };
}

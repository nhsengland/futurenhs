import { Image } from './image'
import { BreadCrumbList } from './routing'
import { User } from './user'

export interface Folder {
    id: string
    type: 'folder' | 'file'
    text: {
        name: string
        body?: string
    }
    modified?: string
    path?: BreadCrumbList
}

export interface FolderContent {
    id: string
    type: 'folder' | 'file'
    extension?: string
    name: string
    modified?: string
    modifiedBy?: Partial<User>
    created?: string
    createdBy?: Partial<User>
    lastUpdated?: Partial<User>
    text?: {
        body?: string
    }
    path?: BreadCrumbList
    downloadLink?: string
    versions?: Array<FileVersion>
}

export interface FileVersion {
    id: string
    name: string
    modifiedAt: string
    size: number
    lastUpdated?: Partial<User>
}

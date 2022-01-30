import { GroupPage } from '@appTypes/page';
import { Folder, FolderContent } from '@appTypes/file';
import { GroupsPageTextContent } from '@appTypes/content';

declare interface ContentText extends GroupsPageTextContent {
    foldersHeading: string; 
    noFolders: string; 
    createFolder: string;
    updateFolder: string;
    deleteFolder: string;
    createFile: string;
}

export interface Props extends GroupPage {
    contentText: ContentText;
    folderId: string;
    folder: Folder;
    folderContents: Array<FolderContent>;
}
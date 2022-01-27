import { GroupPage } from '@appTypes/page';
import { Folder, FolderContent } from '@appTypes/file';

export interface Props extends GroupPage {
    folderId: string;
    folder: Folder;
    folderContents: Array<FolderContent>;
}
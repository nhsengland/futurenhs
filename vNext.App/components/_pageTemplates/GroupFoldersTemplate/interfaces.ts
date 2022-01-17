import { GroupPage } from '@appTypes/page';
import { Folder } from '@appTypes/file';

export interface Props extends GroupPage {
    folderId: string;
    folder: Folder;
    files: Array<any>;
}
import { Folder } from '@appTypes/file';
import { GroupPage } from '@appTypes/page';

export interface Props extends GroupPage {
    folderId: string;
    folder?: Folder;
}
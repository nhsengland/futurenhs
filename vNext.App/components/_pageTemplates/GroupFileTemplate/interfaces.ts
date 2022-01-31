import { FolderContent } from '@appTypes/file';
import { GroupPage } from '@appTypes/page';

export interface Props extends GroupPage {
    fileId: string;
    file: FolderContent;
}
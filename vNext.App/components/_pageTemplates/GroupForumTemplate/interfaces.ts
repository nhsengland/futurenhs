import { GroupPage } from '@appTypes/page';
import { Discussion } from '@appTypes/discussion';

export interface Props extends GroupPage {
    discussionsList: Array<Discussion>;
}
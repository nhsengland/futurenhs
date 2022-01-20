import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';
import { User } from '@appTypes/user';

interface Text extends GenericPageTextContent {}

export interface Props extends Page {
    user: User;
    text: Text;
}
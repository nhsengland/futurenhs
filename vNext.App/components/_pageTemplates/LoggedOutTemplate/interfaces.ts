import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';

interface Text extends GenericPageTextContent {}

export interface Props extends Page {
    text: Text;
    logOutUrl: string;
}
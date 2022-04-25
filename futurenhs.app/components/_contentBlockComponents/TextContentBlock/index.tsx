import classNames from 'classnames'
import { Heading } from '@components/Heading'
import { RichText } from '@components/RichText'

import { Props } from './interfaces'

export const TextContentBlock: (props: Props) => JSX.Element = ({
    id,
    text,
    headingLevel,
    className,
}) => {

    const { heading, bodyHtml } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames(className)
    };

    return (
        <div id={id} className={generatedClasses.wrapper}>
            <Heading headingLevel={headingLevel}>{heading}</Heading>
            <RichText bodyHtml={bodyHtml} />
        </div>
    )
}

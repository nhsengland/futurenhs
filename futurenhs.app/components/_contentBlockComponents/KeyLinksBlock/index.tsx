import classNames from 'classnames'
import { Heading } from '@components/Heading'

import { Props } from './interfaces'

export const KeyLinksBlock: (props: Props) => JSX.Element = ({
    id,
    text,
    headingLevel,
    className,
}) => {

    const { heading } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames(className),
        heading: classNames('nhsuk-heading-m')
    };

    return (
        <div id={id} className={generatedClasses.wrapper}>
            <Heading headingLevel={headingLevel} className={generatedClasses.heading}>{heading}</Heading>
            
        </div>
    )
}

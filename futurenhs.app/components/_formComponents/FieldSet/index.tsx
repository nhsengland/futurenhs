import classNames from 'classnames';

import { RichText } from '@components/RichText';

import { Props } from './interfaces';

export const FieldSet: (props: Props) => JSX.Element = ({
    input: {
        name
    },
    text,
    children,
    className
}) => {

    const { legend, hint } = text ?? {};

    const generatedClasses: any = {
        wrapper: classNames('nhsuk-fieldset', className),
        legend: classNames('nhsuk-fieldset__legend'),
        hint: classNames('c-hint')
    };

    return (

        <fieldset id={name} className={generatedClasses.wrapper}>
            <legend className={generatedClasses.legend}>{legend}</legend>
            {hint &&
                <RichText
                    className={generatedClasses.hint}
                    bodyHtml={hint}
                    wrapperElementType="span" />
            }
            {children}
        </fieldset>

    )

}

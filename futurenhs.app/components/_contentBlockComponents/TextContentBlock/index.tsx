import classNames from 'classnames'

import { useCsrf } from '@hooks/useCsrf'
import { useFormConfig } from '@hooks/useForm'
import { Heading } from '@components/Heading'
import { RichText } from '@components/RichText'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Form } from '@components/Form'
import { formTypes } from '@constants/forms'

import { Props } from './interfaces'
import { FormConfig } from '@appTypes/form'

export const TextContentBlock: (props: Props) => JSX.Element = ({
    id,
    block,
    headingLevel,
    isEditable,
    changeAction,
    initialErrors,
    className,
}) => {

    const { title, mainText } = block?.content ?? {};

    const csrfToken: string = useCsrf();
    const formConfig: FormConfig = useFormConfig(formTypes.CONTENT_BLOCK_TEXT, {
        [`title-${id}`]: title,
        [`mainText-${id}`]: mainText
    }, initialErrors);

    const generatedClasses: any = {
        wrapper: classNames(className),
        heading: classNames('nhsuk-heading-l', 'u-mb-14')
    };

    const handleChange = (formState: Record<any, any>) => {

        changeAction?.({ instanceId: id, formState })

    }

    if(isEditable){

        return (

            <LayoutColumnContainer>
                <LayoutColumn desktop={9}>
                    <Form
                        csrfToken={csrfToken}
                        instanceId={id}
                        formConfig={formConfig}
                        shouldRenderSubmitButton={false}
                        changeAction={handleChange} />
                </LayoutColumn>
            </LayoutColumnContainer>

        )

    }

    return (
        <div id={id} className={generatedClasses.wrapper}>
            <Heading headingLevel={headingLevel} className={generatedClasses.heading}>{title}</Heading>
            <RichText bodyHtml={mainText} wrapperElementType="div" />
        </div>
    )
}

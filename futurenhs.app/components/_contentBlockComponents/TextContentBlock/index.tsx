import classNames from 'classnames'

import { simpleClone } from '@helpers/util/data'
import { useCsrf } from '@hooks/useCsrf'
import { useFormConfig } from '@hooks/useForm'
import { Heading } from '@components/Heading'
import { RichText } from '@components/RichText'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Form } from '@components/Form'
import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'
import { CmsContentBlock } from '@appTypes/contentBlock';

import { Props } from './interfaces'

export const TextContentBlock: (props: Props) => JSX.Element = ({
    block,
    headingLevel,
    isEditable,
    changeAction,
    initialErrors,
    className,
}) => {

    const blockId: string = block?.item?.id;
    const { title, mainText } = block?.content ?? {};

    const csrfToken: string = useCsrf();
    const formConfig: FormConfig = useFormConfig(formTypes.CONTENT_BLOCK_TEXT, {
        [`title-${blockId}`]: title,
        [`mainText-${blockId}`]: mainText
    }, initialErrors[blockId] ?? {});

    const generatedClasses: any = {
        wrapper: classNames(className),
        heading: classNames('nhsuk-heading-l', 'u-mb-14')
    };

    const handleChange = ({ values, errors, visited }): void => {

        const updatedBlock: CmsContentBlock = simpleClone(block);

        /**
         * Handle value updates
         */
        if (updatedBlock?.content) {

            Object.keys(updatedBlock.content).forEach((key) => {

                const fieldName: string = `${key}-${blockId}`
                const value: any = values[fieldName];

                /**
                 * If a field has been interacted with
                 */
                if (visited[fieldName] && updatedBlock.content[key] !== value) {

                    updatedBlock.content[key] = value ?? null;

                }

            });

        }

        changeAction?.({ block: updatedBlock, errors })

    }

    if(isEditable){

        return (

            <LayoutColumnContainer>
                <LayoutColumn desktop={9}>
                    <Form
                        csrfToken={csrfToken}
                        instanceId={blockId}
                        formConfig={formConfig}
                        shouldRenderSubmitButton={false}
                        changeAction={handleChange} />
                </LayoutColumn>
            </LayoutColumnContainer>

        )

    }

    return (
        <div id={blockId} className={generatedClasses.wrapper}>
            <Heading headingLevel={headingLevel} className={generatedClasses.heading}>{title}</Heading>
            <RichText bodyHtml={mainText} wrapperElementType="div" />
        </div>
    )
}

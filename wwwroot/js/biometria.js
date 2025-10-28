// Espera o documento carregar
document.addEventListener("DOMContentLoaded", function () {

    const selectColaborador = document.getElementById("selectColaborador");
    const btnCadastrarBiometria = document.getElementById("btnCadastrarBiometria");
    const statusBiometria = document.getElementById("statusBiometria");

    if (!btnCadastrarBiometria) {
        return; // Sai se o botão não estiver na página
    }

    btnCadastrarBiometria.addEventListener("click", async function () {

        const colaboradorId = parseInt(selectColaborador.value, 10);

        // 1. Validação
        if (!colaboradorId || colaboradorId === 0) {
            exibirMensagem("Por favor, selecione um colaborador primeiro.", "danger");
            return;
        }

        // Desabilita o botão para evitar cliques duplos
        btnCadastrarBiometria.disabled = true;

        try {
            // 2. Chamar o leitor local
            // Este é o endpoint que o software do leitor (ex: ControliD) expõe localmente.
            // Pode variar (ex: /capture, /get_template, etc.)
            const leitorUrl = "http://localhost:8000/capture";

            exibirMensagem("Aguardando leitor... Por favor, posicione o dedo.", "info");

            const responseLeitor = await fetch(leitorUrl, {
                method: "GET",
                mode: "cors" // Necessário para chamadas localhost
            });

            if (!responseLeitor.ok) {
                throw new Error(`Falha ao conectar no leitor. (Status: ${responseLeitor.status})`);
            }

            const dataLeitor = await responseLeitor.json();

            // Assumindo que o leitor retorna um JSON { success: true, template: "..." }
            if (!dataLeitor.success || !dataLeitor.template) {
                throw new Error(dataLeitor.message || "Leitor não retornou um template válido.");
            }

            const templateBase64 = dataLeitor.template;
            exibirMensagem("Digital capturada! Salvando no sistema...", "info");

            // 3. Enviar para o servidor do Portal (Razor Page Handler)
            const requestBody = {
                ColaboradorId: colaboradorId,
                BiometriaTemplateBase64: templateBase64
            };

            // Precisamos do RequestVerificationToken para POSTs no Razor Pages
            const antiforgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

            const responseServidor = await fetch("?handler=CadastrarBiometria", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": antiforgeryToken
                },
                body: JSON.stringify(requestBody)
            });

            const dataServidor = await responseServidor.json();

            // 4. Exibir resultado final
            if (dataServidor.success) {
                exibirMensagem(dataServidor.message || "Biometria cadastrada com sucesso!", "success");
            } else {
                throw new Error(dataServidor.message || "Erro desconhecido ao salvar a biometria.");
            }

        } catch (error) {
            console.error("Erro no cadastro de biometria:", error);
            let mensagemErro = error.message;

            if (error.message.includes("Failed to fetch")) {
                mensagemErro = "<strong>Falha de conexão.</strong><br>Não foi possível conectar ao software do leitor em <code>http://localhost:9000</code>. Verifique se o software está em execução.";
            }

            exibirMensagem(mensagemErro, "danger");
        } finally {
            // Reabilita o botão
            btnCadastrarBiometria.disabled = false;
        }
    });


    function exibirMensagem(mensagem, tipo) {
        // tipo pode ser: success, info, warning, danger
        statusBiometria.innerHTML = `<div class="alert alert-${tipo}" role="alert">${mensagem}</div>`;
    }

});